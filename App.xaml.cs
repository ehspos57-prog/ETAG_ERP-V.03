using ETAG_ERP.Helpers;
using ETAG_ERP.Views;
using ETAG_ERP.Models;
using System.Windows;

namespace ETAG_ERP
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ✅ إنشاء قاعدة البيانات إذا لم تكن موجودة
            SchemaCreator.CreateDatabaseAndTables();

#if DEBUG
            // فتح MainWindow مباشرة في وضع التطوير
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.MainWindow = mainWindow;
#else
            // فتح نافذة تسجيل الدخول في الوضع العادي (الإصدار النهائي)
            var loginWindow = new ETAG_POS.LoginWindow();
            loginWindow.Show();
            this.MainWindow = loginWindow;
#endif
        }


    }
    public static class SessionManager
    {
        public static User CurrentUser { get; set; }
    }


}
