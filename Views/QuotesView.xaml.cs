using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;

namespace ETAG_ERP.Views
{
    public class QuotesViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // 🔄 إشعار التغييرات
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

        // ✅ خصائص الواجهة
        private ObservableCollection<Invoice> _quoteList;
        public ObservableCollection<Invoice> QuoteList
        {
            get => _quoteList;
            set { _quoteList = value; OnPropertyChanged(nameof(QuoteList)); }
        }

        private Invoice _selectedQuote;
        public Invoice SelectedQuote
        {
            get => _selectedQuote;
            set { _selectedQuote = value; OnPropertyChanged(nameof(SelectedQuote)); }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set { _fromDate = value; OnPropertyChanged(nameof(FromDate)); }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set { _toDate = value; OnPropertyChanged(nameof(ToDate)); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
        }

        public QuotesViewModel()
        {
            QuoteList = new ObservableCollection<Invoice>();
            LoadQuotesFromDatabase();
        }

        // ======================
        // تحميل البيانات
        // ======================
        private void LoadQuotesFromDatabase()
        {
            QuoteList.Clear();

            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                using var cmd = new SQLiteCommand("SELECT * FROM Quotes ORDER BY Date DESC", conn);
                using var reader = cmd.ExecuteReader();

                bool hasData = false;

                while (reader.Read())
                {
                    hasData = true;
                    QuoteList.Add(new Invoice
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        InvoiceNumber = reader["InvoiceNumber"].ToString(),
                        ClientName = reader["ClientName"].ToString(),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        Description = reader["Description"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        Username = reader["Username"].ToString()
                    });
                }

                // لو مفيش بيانات، أضف بيانات تجريبية
                if (!hasData)
                {
                    QuoteList.Add(new Invoice
                    {
                        Id = 1,
                        InvoiceNumber = "Q-001",
                        ClientName = "عميل تجريبي 1",
                        Date = DateTime.Today,
                        Description = "عرض سعر تجريبي",
                        TotalAmount = 1000,
                        Username = "Admin"
                    });
                    QuoteList.Add(new Invoice
                    {
                        Id = 2,
                        InvoiceNumber = "Q-002",
                        ClientName = "عميل تجريبي 2",
                        Date = DateTime.Today.AddDays(-2),
                        Description = "عرض سعر مخصص",
                        TotalAmount = 750,
                        Username = "Admin"
                    });
                }
            }
            catch
            {
                // بيانات احتياطية لو فشل الاتصال بالقاعدة
                QuoteList.Add(new Invoice
                {
                    Id = 1,
                    InvoiceNumber = "Q-001",
                    ClientName = "عميل تجريبي 1",
                    Date = DateTime.Today,
                    Description = "عرض سعر تجريبي",
                    TotalAmount = 1000,
                    Username = "Admin"
                });
                QuoteList.Add(new Invoice
                {
                    Id = 2,
                    InvoiceNumber = "Q-002",
                    ClientName = "عميل تجريبي 2",
                    Date = DateTime.Today.AddDays(-2),
                    Description = "عرض سعر مخصص",
                    TotalAmount = 750,
                    Username = "Admin"
                });
            }
        }

        // ======================
        // تنفيذ الأوامر
        // ======================
        private void ExecuteFilter()
        {
            var filtered = QuoteList.Where(q =>
                (!FromDate.HasValue || q.Date >= FromDate.Value) &&
                (!ToDate.HasValue || q.Date <= ToDate.Value) &&
                (string.IsNullOrEmpty(SearchText) || q.InvoiceNumber.Contains(SearchText) || q.Description.Contains(SearchText))
            ).ToList();

            QuoteList.Clear();
            foreach (var q in filtered)
                QuoteList.Add(q);
        }

        private void ExecuteAddQuote()
        {
            MessageBox.Show("فتح نموذج إضافة عرض سعر جديد.");
            // هنا ممكن تفتح نافذة AddQuoteWindow
        }

        private void ExecuteEditQuote()
        {
            if (SelectedQuote == null)
            {
                MessageBox.Show("اختر عرض سعر لتعديله.");
                return;
            }

            MessageBox.Show($"تعديل عرض السعر: {SelectedQuote.InvoiceNumber}");
            // هنا ممكن تفتح نافذة EditQuoteWindow
        }

        private void ExecuteDeleteQuote()
        {
            if (SelectedQuote == null)
            {
                MessageBox.Show("اختر عرض سعر للحذف.");
                return;
            }

            if (MessageBox.Show($"هل تريد حذف عرض السعر: {SelectedQuote.InvoiceNumber}؟", "تأكيد الحذف", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using var conn = DatabaseHelper.GetConnection();
                    conn.Open();
                    using var cmd = new SQLiteCommand("DELETE FROM Quotes WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", SelectedQuote.Id);
                    cmd.ExecuteNonQuery();

                    QuoteList.Remove(SelectedQuote);
                    SelectedQuote = null;
                    MessageBox.Show("تم الحذف بنجاح ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}");
                }
            }
        }

        private void ExecuteRefresh() => LoadQuotesFromDatabase();

        private void ExecuteExport()
        {
            MessageBox.Show("تم التصدير إلى Excel/PDF (placeholder).");
        }

        private void ExecutePrint()
        {
            MessageBox.Show("تمت الطباعة (placeholder).");
        }

        private void ExecuteOpenDetails()
        {
            if (SelectedQuote == null)
            {
                MessageBox.Show("اختر عرض سعر لعرض التفاصيل.");
                return;
            }

            var detailsWindow = new InvoiceDetailsWindow(SelectedQuote);
            detailsWindow.Show();
        }
    }
}
