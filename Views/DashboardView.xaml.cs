using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class DashboardView : UserControl
    {
        // نماذج بيانات للعرض
        public ObservableCollection<Deal> TopDeals { get; set; }
        public ObservableCollection<ClientSummary> TopClients { get; set; }
        public ObservableCollection<SupplierSummary> TopSuppliers { get; set; }

        public DashboardView()
        {
            InitializeComponent();

            // بيانات مبدئية تجريبية (Mock Data)
            LoadDashboardData();

            // ربط الـ DataGrids
            TopDealsGrid.ItemsSource = TopDeals;
            TopClientsGrid.ItemsSource = TopClients;
            TopSuppliersGrid.ItemsSource = TopSuppliers;
        }

        private void LoadDashboardData()
        {
            try
            {
                // هنا في الواقع هتجيب من DatabaseHelper
                // KPIs
                TotalContractsValue.Text = "12";
                WonTendersValue.Text = "1,450,000 ج";
                TotalSalesValue.Text = "2,300,000 ج";
                TotalExpensesValue.Text = "950,000 ج";
                NetProfitValue.Text = "1,350,000 ج";

                // توزيع العمليات
                DealsCount.Text = "5 عمليات";
                DealsValue.Text = "1,200,000 ج";

                TendersCount.Text = "3 عمليات";
                TendersValue.Text = "900,000 ج";

                SuppliesCount.Text = "2 عمليات";
                SuppliesValue.Text = "500,000 ج";

                CashSalesCount.Text = "15 فاتورة";
                CashSalesValue.Text = "350,000 ج";

                CreditSalesCount.Text = "8 فواتير";
                CreditSalesValue.Text = "250,000 ج";

                // الذمم
                ReceivablesValue.Text = "1,800,000 ج";
                OverdueReceivablesCount.Text = "متأخر: 3";

                PayablesValue.Text = "1,200,000 ج";
                OverduePayablesCount.Text = "متأخر: 1";

                OpenServiceOrders.Text = "7 أوامر";
                ServiceDueSoon.Text = "مستحق قريبًا: 2";

                CriticalStockItems.Text = "6 أصناف";

                // جداول: Top Deals
                TopDeals = new ObservableCollection<Deal>
                {
                    new Deal { Name="صفقة ضخ هيدروليك", Client="شركة النصر", Status="جاري", Amount="750,000 ج", DueDate="2025-09-10" },
                    new Deal { Name="مناقصة وزارة الكهرباء", Client="الوزارة", Status="فاز", Amount="1,200,000 ج", DueDate="2025-12-15" },
                    new Deal { Name="توريد مضخات", Client="شركة المقاولون العرب", Status="تم", Amount="500,000 ج", DueDate="2025-07-05" },
                };

                // جداول: Top Clients
                TopClients = new ObservableCollection<ClientSummary>
                {
                    new ClientSummary { Name="شركة النصر", Total="2,000,000 ج", LastDate="2025-08-10" },
                    new ClientSummary { Name="شركة البترول", Total="1,500,000 ج", LastDate="2025-08-15" },
                    new ClientSummary { Name="الهيئة الهندسية", Total="1,200,000 ج", LastDate="2025-08-20" },
                };

                // جداول: Top Suppliers
                TopSuppliers = new ObservableCollection<SupplierSummary>
                {
                    new SupplierSummary { Name="شركة SKF للمحامل", Total="1,000,000 ج", LastDate="2025-08-12" },
                    new SupplierSummary { Name="شركة Parker", Total="800,000 ج", LastDate="2025-08-14" },
                    new SupplierSummary { Name="شركة Bosch Rexroth", Total="600,000 ج", LastDate="2025-08-18" },
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تحميل البيانات: " + ex.Message, "Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // موديلات صغيرة للعرض
    public class Deal
    {
        public string Name { get; set; }
        public string Client { get; set; }
        public string Status { get; set; }
        public string Amount { get; set; }
        public string DueDate { get; set; }
    }

    public class ClientSummary
    {
        public string Name { get; set; }
        public string Total { get; set; }
        public string LastDate { get; set; }
    }

    public class SupplierSummary
    {
        public string Name { get; set; }
        public string Total { get; set; }
        public string LastDate { get; set; }
    }
}
