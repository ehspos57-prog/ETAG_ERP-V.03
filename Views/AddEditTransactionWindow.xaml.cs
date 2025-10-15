using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace ETAG_ERP.Views
{
    public partial class AddEditTransactionWindow : Window
    {
        private readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ETAG.db");
        public TransactionModel CurrentTransaction { get; private set; }
        public ObservableCollection<string> Types { get; set; }
        public TransactionModel Transaction { get; internal set; }

        public AddEditTransactionWindow(TransactionModel transaction = null)
        {
            InitializeComponent();
            Types = new ObservableCollection<string> { "المبيعات", "المشتريات", "المرتجعات", "عروض الأسعار", "المصروفات", "كشف الحساب" };
            DataContext = this;

            if (transaction != null)
            {
                CurrentTransaction = new TransactionModel
                {
                    Id = transaction.Id,
                    Date = transaction.Date,
                    Type = transaction.Type,
                    InvoiceNumber = transaction.InvoiceNumber,
                    Amount = transaction.Amount,
                    Username = transaction.Username,
                    Description = transaction.Description
                };
            }
            else
            {
                CurrentTransaction = new TransactionModel
                {
                    Date = DateTime.Today,
                    Type = Types.First(),
                    InvoiceNumber = "",
                    Amount = 0,
                    Username = Environment.UserName,
                    Description = ""
                };
            }

            this.DataContext = CurrentTransaction;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentTransaction.Type) ||
                string.IsNullOrEmpty(CurrentTransaction.InvoiceNumber) ||
                CurrentTransaction.Amount <= 0)
            {
                MessageBox.Show("الرجاء تعبئة جميع الحقول بشكل صحيح.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // حفظ البيانات في DB
            if (File.Exists(_dbPath))
            {
                using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                conn.Open();

                if (CurrentTransaction.Id > 0)
                {
                    // تعديل
                    var cmd = new SQLiteCommand("UPDATE Transactions SET Date=@Date, Type=@Type, InvoiceNumber=@InvoiceNumber, Amount=@Amount, Username=@Username, Description=@Description WHERE Id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Date", CurrentTransaction.Date);
                    cmd.Parameters.AddWithValue("@Type", CurrentTransaction.Type);
                    cmd.Parameters.AddWithValue("@InvoiceNumber", CurrentTransaction.InvoiceNumber);
                    cmd.Parameters.AddWithValue("@Amount", CurrentTransaction.Amount);
                    cmd.Parameters.AddWithValue("@Username", CurrentTransaction.Username);
                    cmd.Parameters.AddWithValue("@Description", CurrentTransaction.Description);
                    cmd.Parameters.AddWithValue("@Id", CurrentTransaction.Id);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // إضافة جديدة
                    var cmd = new SQLiteCommand("INSERT INTO Transactions (Date, Type, InvoiceNumber, Amount, Username, Description) VALUES (@Date,@Type,@InvoiceNumber,@Amount,@Username,@Description); SELECT last_insert_rowid();", conn);
                    cmd.Parameters.AddWithValue("@Date", CurrentTransaction.Date);
                    cmd.Parameters.AddWithValue("@Type", CurrentTransaction.Type);
                    cmd.Parameters.AddWithValue("@InvoiceNumber", CurrentTransaction.InvoiceNumber);
                    cmd.Parameters.AddWithValue("@Amount", CurrentTransaction.Amount);
                    cmd.Parameters.AddWithValue("@Username", CurrentTransaction.Username);
                    cmd.Parameters.AddWithValue("@Description", CurrentTransaction.Description);
                    CurrentTransaction.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
