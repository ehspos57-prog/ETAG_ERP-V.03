using ETAG_ERP.Helpers; // InventoryExportHelper
using ETAG_ERP.ViewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadContent(new SalesView()); // فتح شاشة البيع تلقائياً عند البداية
        }

        // الدالة العامة لتحميل أي UserControl في ContentArea
        private void LoadContent(UserControl control)
        {
            try
            {
                ContentArea.Content = control;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل الشاشة:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void فاتورة_جديدة_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "تأكد من حفظ الفواتير الحالية من شاشة البيع قبل الإغلاق.\nهل أنت متأكد من رغبتك في فتح فاتورة جديدة؟",
                "تأكيد فتح فاتورة جديدة",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                LoadContent(new SalesView());
            }
        }

        // واجهة موحدة لأي ViewModel عنده عملية حفظ
        public interface ISavable
        {
            Task SaveAsync();
            Task SaveAllChangesAsync();
        }

        private async 
        Task
SaveAllChangesAsync()
        {
            if (ContentArea.Content is FrameworkElement element &&
                element.DataContext is ISavable savable)
            {
                await savable.SaveAllChangesAsync();
                MessageBox.Show("تم حفظ البيانات بنجاح");
            }
            else
            {
                MessageBox.Show("هذه الشاشة لا تدعم الحفظ.");
            }
        }


        private async void إغلاق_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("هل أنت متأكد من تسجيل الخروج؟", "تأكيد تسجيل الخروج",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await SaveAllChangesAsync();
                    MessageBox.Show("تم حفظ التعديلات بنجاح.", "حفظ البيانات", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
                catch
                {
                    // الخطأ معروض مسبقاً
                }
            }
        }

        // أزرار القائمة الجانبية
        private void الأصناف_Click(object sender, RoutedEventArgs e) => LoadContent(new CategoryView());
        private void العملاء_Click(object sender, RoutedEventArgs e) => LoadContent(new CustomersView());
        private void فاتورة_Click(object sender, RoutedEventArgs e) => LoadContent(new SalesView());
        private void الحسابات_Click(object sender, RoutedEventArgs e) => LoadContent(new AccountsView());
        private void المرتجعات_Click(object sender, RoutedEventArgs e) => LoadContent(new ReturnsView());
        private void كشف_حساب_Click(object sender, RoutedEventArgs e) => LoadContent(new LedgerView());
        private void Expenses_Click(object sender, RoutedEventArgs e) => LoadContent(new AddEditExpenseControl());
        private void تقارير_Click(object sender, RoutedEventArgs e) => LoadContent(new DashboardView());
        private void مندوبين_Click(object sender, RoutedEventArgs e) => LoadContent(new EmployeesSchedulerView());


        // النوافذ المنبثقة
        private void عروض_الأسعار_Click(object sender, RoutedEventArgs e)
        {
            var window = new PriceQuoteView();
            window.Show();
        }

        private void إذن_الاستلام_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReceivingNoteView();
            window.Show();
        }

        private void الإعدادات_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsView();
            window.Show();
        }

        private void البوابة_Click(object sender, RoutedEventArgs e)
        {
            var webWindow = new Window
            {
                Title = "بوابة الضرائب المصرية",
                Width = 1100,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var browser = new WebBrowser();
            browser.Navigate("https://invoicing.eta.gov.eg/");

            webWindow.Content = browser;
            webWindow.Show();
        }

        private void جرد_المخزون_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "هل تريد تصدير جرد المخزون؟\n(نعم PDF، لا إغلاق، إلغاء Excel)",
                "جرد المخزون",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                InventoryExportHelper.ExportInventoryToPdf();
            else if (result == MessageBoxResult.Cancel)
                InventoryExportHelper.ExportInventoryToExcel();
        }
    }
}
