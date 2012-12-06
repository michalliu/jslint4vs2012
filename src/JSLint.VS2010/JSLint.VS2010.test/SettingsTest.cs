using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSLint.VS2010.OptionClasses;
using System.IO;

namespace JSLint.VS2010.test
{
    [TestClass]
    public class SettingsTest
    {
        public Options LoadOptionsFromEmbeddedResource(string name)
        {
            using (Stream s = typeof(SettingsTest).Assembly.GetManifestResourceStream("JSLint.VS2010.test." + name))
            {
                OptionsSerializer serializer = new OptionsSerializer();
                return serializer.Deserialize(s);
            }
        }

        [TestMethod]
        public void TestLoadingAllErrors1_2_4()
        {
            // no longer upgraded - just testing it doesn''t err

            Options options = LoadOptionsFromEmbeddedResource("SettingFiles._1_2_4.ALLErrors.xml");
        }

        [TestMethod]
        public void TestLoadingNoErrors1_2_4()
        {
            // no longer upgraded - just testing it doesn''t err

            Options options = LoadOptionsFromEmbeddedResource("SettingFiles._1_2_4.NOErrors.xml");
        }
    }
}
