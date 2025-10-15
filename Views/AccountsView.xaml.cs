using ClosedXML.Excel;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class AccountsView : UserControl
    {
        private readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ETAG.db");
        private bool IsUseDb = true;

        public ObservableCollection<TransactionModel> Transactions { get; set; } = new();
        public ObservableCollection<Client> Clients { get; set; } = new();
        public ObservableCollection<Invoice> SelectedClientInvoices { get; set; } = new();

        public Client SelectedClient { get; set; }
        public DateTime? FromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime? ToDate { get; set; } = DateTime.Now;

        public ObservableCollection<string> TransactionTypes { get; set; } = new()
        { "المبيعات", "المشتريات", "المرتجعات", "عروض الأسعار", "المصروفات", "كشف الحساب" };
        public string SelectedType { get; set; }

        public string SearchText { get; set; }
        public int SelectedTabIndex { get; set; }

        public TransactionModel SelectedTransaction { get; set; }

        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalReturns { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal CurrentBalance { get; set; }

        public AccountsView()
        {
            InitializeComponent();
            DataContext = this;
            LoadAllData();
        }

        #region Data Loading & Summary

        private void LoadAllData()
        {
            Transactions.Clear();
            Clients.Clear();
            SelectedClientInvoices.Clear();

            if (IsUseDb && File.Exists(_dbPath))
            {
                using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                conn.Open();

                // العملاء
                using var cmdClients = new SQLiteCommand("SELECT Id, Name FROM Clients", conn);
                using var readerC = cmdClients.ExecuteReader();
                while (readerC.Read())
                    Clients.Add(new Client { Id = readerC.GetInt32(0), Name = readerC.GetString(1) });

                // الحركات
                using var cmdTrans = new SQLiteCommand("SELECT Date, Type, InvoiceNumber, Amount, Username, Description FROM Transactions", conn);
                using var readerT = cmdTrans.ExecuteReader();
                while (readerT.Read())
                    Transactions.Add(new TransactionModel
                    {
                        Date = readerT.GetDateTime(0),
                        Type = readerT.GetString(1),
                        InvoiceNumber = readerT.GetString(2),
                        Amount = readerT.GetDecimal(3),
                        Username = readerT.GetString(4),
                        Description = readerT.GetString(5)
                    });
            }

            // بيانات تجريبية لو DB مش موجودة أو فارغة
            if (!Transactions.Any())
            {
                Transactions.Add(new TransactionModel { Date = DateTime.Today, Type = "المبيعات", InvoiceNumber = "INV001", Amount = 5000, Username = "admin", Description = "بيع قطع غيار" });
                Transactions.Add(new TransactionModel { Date = DateTime.Today.AddDays(-1), Type = "المشتريات", InvoiceNumber = "PUR001", Amount = 3000, Username = "admin", Description = "شراء مواد خام" });
                Transactions.Add(new TransactionModel { Date = DateTime.Today.AddDays(-2), Type = "المرتجعات", InvoiceNumber = "RET001", Amount = 1500, Username = "admin", Description = "مرتجع قطع" });

                Clients.Add(new Client { Id = 1, Name = "شركة ألف" });
                Clients.Add(new Client { Id = 2, Name = "شركة باء" });
            }

            SelectedClient ??= Clients.FirstOrDefault();
            LoadClientInvoices();
            CalculateSummary();
        }

        private void LoadClientInvoices()
        {
            SelectedClientInvoices.Clear();
            if (SelectedClient == null) return;

            // بيانات فواتير تجريبية
            SelectedClientInvoices.Add(new Invoice { InvoiceNumber = "INV-001", Date = DateTime.Today.AddDays(-10), Status = "مدفوعة", TotalAmount = 5000 });
            SelectedClientInvoices.Add(new Invoice { InvoiceNumber = "INV-002", Date = DateTime.Today.AddDays(-5), Status = "قيد الدفع", TotalAmount = 3000 });
        }

        private void CalculateSummary()
        {
            TotalSales = Transactions.Where(t => t.Type == "المبيعات").Sum(t => t.Amount);
            TotalPurchases = Transactions.Where(t => t.Type == "المشتريات").Sum(t => t.Amount);
            TotalReturns = Transactions.Where(t => t.Type == "المرتجعات").Sum(t => t.Amount);
            TotalExpenses = 0;
            NetProfit = TotalSales - TotalPurchases - TotalReturns - TotalExpenses;
            CurrentBalance = NetProfit;
        }

        #endregion

        #region Button Events

        private void FilterTransactions_Click(object sender, RoutedEventArgs e)
        {
            var filtered = Transactions
                .Where(t =>
                    (!FromDate.HasValue || t.Date >= FromDate) &&
                    (!ToDate.HasValue || t.Date <= ToDate) &&
                    (string.IsNullOrEmpty(SelectedType) || t.Type == SelectedType) &&
                    (string.IsNullOrEmpty(SearchText) || t.Description?.Contains(SearchText) == true)
                ).ToList();

            Transactions.Clear();
            foreach (var t in filtered) Transactions.Add(t);
            CalculateSummary();
        }

        private void ExportTransactions_Click(object sender, RoutedEventArgs e)
        {
            if (!Transactions.Any())
            {
                MessageBox.Show("لا توجد بيانات لتصديرها.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Transactions");

            ws.Cell(1, 1).Value = "التاريخ";
            ws.Cell(1, 2).Value = "النوع";
            ws.Cell(1, 3).Value = "رقم الفاتورة";
            ws.Cell(1, 4).Value = "المبلغ";
            ws.Cell(1, 5).Value = "المستخدم";
            ws.Cell(1, 6).Value = "الوصف";

            int row = 2;
            foreach (var t in Transactions)
            {
                ws.Cell(row, 1).Value = t.Date.ToString("yyyy-MM-dd");
                ws.Cell(row, 2).Value = t.Type;
                ws.Cell(row, 3).Value = t.InvoiceNumber;
                ws.Cell(row, 4).Value = t.Amount;
                ws.Cell(row, 5).Value = t.Username;
                ws.Cell(row, 6).Value = t.Description;
                row++;
            }

            ws.Columns().AdjustToContents();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"Transactions_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            wb.SaveAs(path);
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        private void PrintTransactions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("سيتم الطباعة باستخدام FastReport (ضع القالب لاحقاً)");
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            var newTrans = new TransactionModel
            {
                Date = DateTime.Now,
                Type = "المبيعات",
                InvoiceNumber = $"INV{Transactions.Count + 1:000}",
                Amount = 1000,
                Username = "admin",
                Description = "بيع جديد"
            };

            if (IsUseDb && File.Exists(_dbPath))
            {
                using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                conn.Open();
                using var cmd = new SQLiteCommand(conn)
                {
                    CommandText = "INSERT INTO Transactions(Date, Type, InvoiceNumber, Amount, Username, Description) VALUES(@date,@type,@inv,@amt,@user,@desc)"
                };
                cmd.Parameters.AddWithValue("@date", newTrans.Date);
                cmd.Parameters.AddWithValue("@type", newTrans.Type);
                cmd.Parameters.AddWithValue("@inv", newTrans.InvoiceNumber);
                cmd.Parameters.AddWithValue("@amt", newTrans.Amount);
                cmd.Parameters.AddWithValue("@user", newTrans.Username);
                cmd.Parameters.AddWithValue("@desc", newTrans.Description);
                cmd.ExecuteNonQuery();
            }

            Transactions.Add(newTrans);
            CalculateSummary();
        }

        private void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTransaction == null) return;

            SelectedTransaction.Amount += 100;
            CalculateSummary();

            if (IsUseDb && File.Exists(_dbPath))
            {
                using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                conn.Open();
                using var cmd = new SQLiteCommand("UPDATE Transactions SET Amount=@amt WHERE InvoiceNumber=@inv", conn);
                cmd.Parameters.AddWithValue("@amt", SelectedTransaction.Amount);
                cmd.Parameters.AddWithValue("@inv", SelectedTransaction.InvoiceNumber);
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTransaction == null) return;

            if (MessageBox.Show($"هل تريد حذف الحركة رقم {SelectedTransaction.InvoiceNumber}؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            if (IsUseDb && File.Exists(_dbPath))
            {
                using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                conn.Open();
                using var cmd = new SQLiteCommand("DELETE FROM Transactions WHERE InvoiceNumber=@inv", conn);
                cmd.Parameters.AddWithValue("@inv", SelectedTransaction.InvoiceNumber);
                cmd.ExecuteNonQuery();
            }

            Transactions.Remove(SelectedTransaction);
            SelectedTransaction = null;
            CalculateSummary();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAllData();
        }

        private void OpenInvoiceDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Invoice invoice)
            {
                var window = new InvoiceDetailsWindow(invoice);
                window.Show();
            }
        }

        private void ClientsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is Client client)
            {
                SelectedClient = client;
                LoadClientInvoices();
            }
        }

        #endregion
    }
}
