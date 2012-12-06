using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSLint.VS2010.OptionClasses
{
    public interface IOptionsProvider
    {
		String Name { get; }
        Options GetOptions();
        void Save(Options options);
        bool IsReadOnly { get; }
        void Refresh();
    }
}
