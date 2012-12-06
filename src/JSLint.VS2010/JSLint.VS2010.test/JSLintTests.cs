using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSLint.VS2010.LinterBridge;
using JSLint.VS2010.OptionClasses;

namespace JSLint.VS2010.test
{
	[TestClass]
	public class JSLintTests
	{
		private JSLinter _linter;
		[TestInitialize]
		public void Setup()
		{
			_linter = new JSLinter();
		}

		[TestCleanup]
		public void TearDown()
		{
			_linter.Dispose();
		}

        public SerializableDictionary<string, bool> GetOptions(string truthy = null, string falsy = null)
        {
            var returner = LintBooleanSettingModel.GetDefaultOptions();
            if (truthy != null)
            {
                foreach (string truthyoption in truthy.Split(' '))
                {
                    returner[truthyoption] = true;
                }
            }
            if (falsy != null)
            {
                foreach (string falsyoption in falsy.Split(' '))
                {
                    returner[falsyoption] = false;
                }
            }

            return returner;
        }

		private void TestLint(string javascript, List<string> errorsExpected, JSLintOptions lintoptions = null, List<string> todos = null, Options options = null)
		{
			if (options == null)
			{
				options = new Options();

				if (lintoptions != null)
				{
					options.JSLintOptions = lintoptions;
				}
			}

			lintoptions = options.JSLintOptions;

			IgnoreErrorSectionsHandler ignoreErrorHandler = new IgnoreErrorSectionsHandler(javascript);

			IEnumerable<JSLintError> errors = _linter.Lint(javascript, lintoptions, true);

			errors = errors.Where(a => !ignoreErrorHandler.IsErrorIgnored(a.Line, a.Column));

            if (lintoptions.FindTodos)
            {
                errors = errors.Concat(TodoFinder.FindTodos(javascript));
            }

			string errorMessage = "got ";

			for(var i = 0; i < errors.Count(); i++) {
				errorMessage += "<" + errors.ElementAt(i).Message + "> ";
			}

			for (var i = 0; i < errorsExpected.Count; i++)
			{
				if (errors.Count() <= i)
				{
					break;
				}
				Assert.AreEqual(errorsExpected[i], errors.ElementAt(i).Message, errorMessage);
			}

			Assert.AreEqual(errorsExpected.Count, errors.Count(), errorMessage);
		}

		[TestMethod]
		public void MissingSemiColon1()
		{
			TestLint(
				"var a\nvar b;", 
				new List<string>() { "Expected ';' and instead saw 'var'."});
		}

		[TestMethod]
		public void MissingSemiColon2()
		{
			TestLint(
				"var a;\nvar b", 
				new List<string>() { "Expected ';' and instead saw '(end)'."});
		}

		[TestMethod]
		public void StrangeEquals()
		{
			TestLint(
				@"var a;
if (a === a) {
	a = true;
}", 
				new List<string>() { "Weird relation."});
		}

		[TestMethod]
		public void MoveVarOutOfFor()
		{
			TestLint(
				@"for (var i = 0; i < 4; i++) { i = 1; }", 
				new List<string>() { "Move 'var' declarations to the top of the function.",
									 "Stopping.  (100% scanned)."});
		}

		[TestMethod]
		public void InPrefix()
		{
			TestLint(
				@"
var a;
if ('e' in a) { a = true; }", 
				new List<string>() { "Unexpected 'in'. Compare with undefined, or use the hasOwnProperty method instead."});
		}

		[TestMethod]
		public void Whitespace1()
		{
			TestLint(
				@"
var i;
for(i = 0; i < 4; i++) { i = 1; }", 
				new List<string>() { "Missing space between 'for' and '('."},
                new JSLintOptions() { BoolOptions2 = GetOptions(falsy : "white.", truthy: "plusplus.") });
		}

		[TestMethod]
		public void TodoDetection1()
		{
			TestLint(
				@"//TODO: first
var i;//TODO second
for (/*TODO third*//*TODO fourth*/i = 0; i < 4; i++) { i = 1; }//           todo five
/* a
b
c
d
 TODO sixth*/
/*
Todo seventh*/
/*TODO*/
//todo",
				new List<string>() { "TODO: first", "TODO second", "TODO third", "TODO fourth", "todo five", "TODO sixth", "Todo seventh", "TODO", "todo" },
                lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy:"white. plusplus."), FindTodos = true });
		}

		[TestMethod]
		public void TodoDetection2()
		{
			TestLint(
				@"/*TODO 1*/function/*TODO 2*/ anon/*TODO 3*/(/*TODO 4*/)/*TODO 5*/ {/*TODO 6*/ var/*TODO 7*/ a/*TODO 8*/ = /*TODO 9*/1,/*TODO 10*/ b/*TODO 11*/ = /*TODO 12*/function/*TODO 13*/(/*TODO 14*/)/*TODO 15*/ {/*TODO 16*/ }/*TODO 17*/;/*TODO 18*/ }/*TODO 19*/",
				getTodos(19),
                lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white."), FindTodos = true });
		}

		private List<string> getTodos(int top)
		{
			List<string> todos = new List<string>();
			int i = 1;
			while (i <= top)
			{
				todos.Add(string.Format("TODO {0}", i++));
			}
			return todos;
		}

		[TestMethod]
		public void TodoDetection3()
		{
			TestLint(
				@"/*TODO 1*/function/*TODO 2*/ c/*TODO 3*/(/*TODO 4*/a/*TODO 5*/,/*TODO 6*/b/*TODO 7*/)/*TODO 8*/{/*TODO 9*/if/*TODO 10*/ (/*TODO 11*/a/*TODO 12*/ ===/*TODO 13*/ b/*TODO 14*/)/*TODO 15*/ {/*TODO 16*/ b =/*TODO 17*/ true; /*TODO 18*/} /*TODO 19*/else/*TODO 20*/ {/*TODO 21*/ return/*TODO 22*/ false;/*TODO 23*/ }/*TODO 24*/}/*TODO 25*/", 
				getTodos(25),
				lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy:"white."), FindTodos = true });
		}

        [TestMethod]
        public void TodoDetection4()
        {
            TestLint(
                @"var todo = 'n'; //not a to-do
// and stodo is not a to-do either
// and todos is not a to-do either",
                new List<string>() { },
                lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white. plusplus."), FindTodos = true });
        }

        [TestMethod]
        public void Unparam()
        {
            TestLint(
                @"
function myfunc(b) {
    var c = 2;
    c += 1;
}",
                new List<string>() { "Unused Variable 'b'." },
                lintoptions: new JSLintOptions() { BoolOptions2 = GetOptions(truthy: "white.", falsy: "unparam"), ErrorOnUnused = true });
        }
	}
}
