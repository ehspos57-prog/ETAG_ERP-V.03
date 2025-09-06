using ClosedXML.Excel;
using ETAG_ERP.Commands;
using ETAG_ERP.Models;
using ETAG_ERP.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ETAG_ERP.Views
{
    public class AccountsViewModel : ETAGBaseViewModel
    {
        private readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ETAG.db");
        private bool IsUseDb = true;

        public ObservableCollection<TransactionModel> Transactions { get; set; } = new();
        public ObservableCollection<Client> Clients { get; set; } = new();
        public ObservableCollection<Invoice> SelectedClientInvoices { get; set; } = new();

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (SetProperty(ref _selectedClient, value))
                    LoadClientInvoices();
            }
        }

        public DateTime? FromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime? ToDate { get; set; } = DateTime.Now;

        public ObservableCollection<string> TransactionTypes { get; set; }
        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                if (SetProperty(ref _selectedType, value))
                    FilterTransactions();
            }
        }

        public string SearchText { get; set; }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (SetProperty(ref _selectedTabIndex, value))
                    FilterTransactions();
            }
        }

        private TransactionModel _selectedTransaction;
        public TransactionModel SelectedTransaction
        {
            get => _selectedTransaction;
            set => SetProperty(ref _selectedTransaction, value);
        }

        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalReturns { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal CurrentBalance { get; set; }

        // Commands
        public ICommand FilterCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand AddTransactionCommand { get; }
        public ICommand EditTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenInvoiceDetailsCommand { get; }

        public AccountsViewModel()
        {
            TransactionTypes = new ObservableCollection<string>
            {
                "المبيعات","المشتريات","المرتجعات","عروض الأسعار","المصروفات","كشف الحساب"
            };
            SelectedType = TransactionTypes.FirstOrDefault();

            FilterCommand = new RelayCommand(FilterTransactions);
            ExportCommand = new RelayCommand(ExecuteExport);
            PrintCommand = new RelayCommand(ExecutePrint);
            AddTransactionCommand = new RelayCommand(ExecuteAddTransaction);
            EditTransactionCommand = new RelayCommand(ExecuteEditTransaction, CanEditOrDelete);
            DeleteTransactionCommand = new RelayCommand(ExecuteDeleteTransaction, CanEditOrDelete);
            RefreshCommand = new RelayCommand(LoadAllData);
            OpenInvoiceDetailsCommand = new RelayCommand<Invoice>(ExecuteOpenInvoiceDetails);

            LoadAllData();
        }

        private void LoadAllData()
        {
            Transactions.Clear();
            Clients.Clear();
            SelectedClientInvoices.Clear();

            if (IsUseDb && File.Exists(_dbPath))
            {
                using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                conn.Open();

                // تحميل العملاء
                using var cmdClients = new SQLiteCommand("SELECT Id, Name FROM Clients", conn);
                using var readerC = cmdClients.ExecuteReader();
                while (readerC.Read())
                    Clients.Add(new Client { Id = readerC.GetInt32(0), Name = readerC.GetString(1) });

                // تحميل الحركات
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

            // بيانات تجريبية لو لا توجد DB
            if (!Transactions.Any())
            {
                Transactions.Add(new TransactionModel { Date = DateTime.Today, Type = "المبيعات", InvoiceNumber = "INV001", Amount = 5000, Username = "admin", Description = "بيع قطع غيار" });
                Transactions.Add(new TransactionModel { Date = DateTime.Today.AddDays(-1), Type = "المشتريات", InvoiceNumber = "PUR001", Amount = 3000, Username = "admin", Description = "شراء مواد خام" });
                Transactions.Add(new TransactionModel { Date = DateTime.Today.AddDays(-2), Type = "المرتجعات", InvoiceNumber = "RET001", Amount = 1500, Username = "admin", Description = "مرتجع قطع" });

                Clients.Add(new Client { Id = 1, Name = "شركة ألف" });
                Clients.Add(new Client { Id = 2, Name = "شركة باء" });
            }

            SelectedClient = Clients.FirstOrDefault();
            CalculateSummary();
        }

        private void FilterTransactions()
        {
            var filtered = Transactions.Where(t =>
                (!FromDate.HasValue || t.Date >= FromDate) &&
                (!ToDate.HasValue || t.Date <= ToDate) &&
                (string.IsNullOrEmpty(SelectedType) || t.Type == SelectedType) &&
                (string.IsNullOrEmpty(SearchText) || (t.Description?.Contains(SearchText) == true))
            ).ToList();

            Transactions = new ObservableCollection<TransactionModel>(filtered);
            CalculateSummary();
        }

        private void ExecuteExport()
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
                ws.Cell(row, 3).Value = t.GetInvoiceNumber();
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

        private void ExecutePrint() => MessageBox.Show("سيتم الطباعة باستخدام FastReport (ضع القالب لاحقاً)");
        private void ExecuteAddTransaction() => MessageBox.Show("إضافة حركة جديدة (لم يتم تنفيذ الواجهة بعد)");
        private bool CanEditOrDelete() => SelectedTransaction != null;
        private void ExecuteEditTransaction() => MessageBox.Show("تعديل الحركة (لم يتم تنفيذ الواجهة بعد)");
        private void ExecuteDeleteTransaction()
        {
            if (SelectedTransaction == null) return;
            if (MessageBox.Show($"هل تريد حذف الحركة رقم {SelectedTransaction.GetInvoiceNumber()}؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Transactions.Remove(SelectedTransaction);
                SelectedTransaction = null;
                CalculateSummary();
            }
        }

        private void LoadClientInvoices()
        {
            SelectedClientInvoices.Clear();
            if (SelectedClient == null) return;

            // بيانات تجريبية للفواتير
            SelectedClientInvoices.Add(new Invoice { InvoiceNumber = "INV-001", Date = DateTime.Today.AddDays(-10), Status = "مدفوعة" });
            SelectedClientInvoices.Add(new Invoice { InvoiceNumber = "INV-002", Date = DateTime.Today.AddDays(-5), Status = "قيد الدفع" });
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

        private void ExecuteOpenInvoiceDetails(Invoice invoice)
        {
            if (invoice != null)
            {
                var window = new InvoiceDetailsWindow(invoice);
                window.Show();
            }
        }
    }

    public class ETAGBaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T field, T value, string propName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
                return true;
            }
            return false;
        }
    }
}
