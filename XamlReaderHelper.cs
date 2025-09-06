using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace ETAG_ERP.Helpers
{
    public static class XamlReaderHelper
    {
        public static object LoadFromString(string xaml)
        {
            using var stringReader = new StringReader(xaml);
            using var xmlReader = XmlReader.Create(stringReader);
            return XamlReader.Load(xmlReader); // System.Windows.Markup.XamlReader.Load
        }

        public static object LoadFromFile(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return XamlReader.Load(stream);
        }
    }
}
