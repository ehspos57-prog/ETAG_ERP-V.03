using System.IO;

namespace ETAG_ERP.Helpers
{
    public class Category
    {
        internal string? Name;

        public int ID { get; internal set; }
        public object Id { get; internal set; }
        public int? ParentID { get; internal set; }
    }
    public static class FileHelper
    {
        public static bool Exists(string path) => File.Exists(path);

        public static void WriteText(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public static string ReadText(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        internal static void ExportInvoiceToPdf(Invoice invoice, string filePath)
        {
            throw new NotImplementedException();
        }
    }

    public static class PrinterHelper
    {
        public static void Print(string content)
        {
            // هنا تضيف منطق الطباعة حسب المشروع
            Console.WriteLine("Printing:\n" + content);
        }
    }

    public static class DataImportExport
    {
        public static void Export<T>(List<T> data, string filePath)
        {
            using (var sw = new StreamWriter(filePath))
            {
                foreach (var item in data)
                    sw.WriteLine(item.ToString());
            }
        }

        public static List<T> Import<T>(string filePath) where T : new()
        {
            var result = new List<T>();
            if (!File.Exists(filePath)) return result;
            // هنا تضيف منطق التحويل حسب النوع
            return result;
        }
    }

    public static class AppSettings
    {
        private static readonly string SettingsFile = "appsettings.json";

        public static void SaveSetting(string key, string value)
        {
            FileHelper.WriteText(SettingsFile, $"{key}={value}");
        }

        public static string? GetSetting(string key)
        {
            var content = FileHelper.ReadText(SettingsFile);
            foreach (var line in content.Split('\n'))
            {
                var parts = line.Split('=');
                if (parts.Length == 2 && parts[0] == key)
                    return parts[1].Trim();
            }
            return null;
        }
    }
}