using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSLint.VS2010.OptionClasses
{
	[Flags]
	public enum IncludeFileType
	{
		None = 0,
		JS = 1,
		HTML = 2,
		CSS = 4,
		Folder = 8
	}
}
