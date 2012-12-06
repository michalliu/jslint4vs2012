﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace JSLint.VS2010.OptionClasses
{
    public class OptionsSerializer
    {
        public void Serialize(Stream stream, Options options)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Options));
            serializer.Serialize(stream, options);
        }

        public Options Deserialize(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Options));
            var options = (Options)serializer.Deserialize(stream);
            return options;
        }
    }
}
