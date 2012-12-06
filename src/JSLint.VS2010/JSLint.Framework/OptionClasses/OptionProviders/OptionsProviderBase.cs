using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSLint.VS2010.OptionClasses.OptionProviders
{
	public abstract class OptionsProviderBase : IOptionsProvider
	{
		protected OptionsProviderBase(String providerName)
		{
			Name = providerName;
		}

		public abstract Options GetOptions();

		public abstract void Save(Options options);

		public abstract bool IsReadOnly { get; }

		public abstract void Refresh();

		public string Name { get; private set; }
	}
}
