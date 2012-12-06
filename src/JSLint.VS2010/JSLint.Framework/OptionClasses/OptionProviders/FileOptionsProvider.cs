﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace JSLint.VS2010.OptionClasses.OptionProviders
{
	public class FileOptionsProvider : OptionsProviderBase
	{
		private String _filePath;
		private Options _options;

		public FileOptionsProvider(String providerName, String filePath)
			: base(providerName)
		{
			_filePath = filePath;
		}

		public override Options GetOptions()
		{
			if (_options == null)
				_options = LoadOptions();
			return _options;
		}

		private Options LoadOptions()
		{
			FileStream fstream = null;
			Options options = null;
			try
			{
				fstream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);

				XmlSerializer serializer = new XmlSerializer(typeof(Options));
				options = (Options)serializer.Deserialize(fstream);
			}
			catch
			{
				options = new Options();
			}
			finally
			{
				if (fstream != null)
					fstream.Dispose();
			}
			return options;
		}

		public override void Save(Options options)
		{
			_options = options;
			using (MemoryStream cloneStream = new MemoryStream())
			using (FileStream fstream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
			{
				OptionsSerializer serializer = new OptionsSerializer();
				serializer.Serialize(cloneStream, options);

				cloneStream.Position = 0;
				cloneStream.CopyTo(fstream);
				cloneStream.Position = 0;
				_options = serializer.Deserialize(cloneStream);
			}
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override void Refresh()
		{
			_options = null;
		}
	}
}
