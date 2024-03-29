using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JSLint.VS2010
{
    public static class Utility
    {
        internal static readonly string AppDataDir =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static readonly Encoding Encoding = Encoding.GetEncoding("iso-8859-1");

        public static string GetFilename(string name)
        {
            return Path.Combine(AppDataDir, name);
        }

        private static readonly Type type = typeof(Utility);

        public static string ReadResourceFile(string name)
        {
            using (StreamReader sr = new StreamReader(
                type.Assembly.GetManifestResourceStream("JSLint.Framework." + name),
                Encoding))
            {
                return sr.ReadToEnd();
            }
        }
    }
}