using ETAG_ERP.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class PriceQuoteViewModel : INotifyPropertyChanged
{
    private readonly string _connectionString = "Data Source=etag_erp.db;Version=3;";

    public ObservableCollection<Client> Clients { get; set; } = new();
    public ObservableCollection<QuoteItem> QuoteItems { get; set; } = new();

    private Client _selectedClient;
    public Client SelectedClient
    {
        get => _selectedClient;
        set { _selectedClient = value; OnPropertyChanged(); }
    }

    private DateTime _quoteDate = DateTime.Now;
    public DateTime QuoteDate
    {
        get => _quoteDate;
        set { _quoteDate = value; OnPropertyChanged(); }
    }

    private string _paymentMethod = "نقدًا";
    public string PaymentMethod
    {
        get => _paymentMethod;
        set { _paymentMethod = value; OnPropertyChanged(); }
    }

    private decimal _discount;
    public decimal Discount
    {
        get => _discount;
        set { _discount = value; OnPropertyChanged(); UpdateTotals(); }
    }

    private decimal _total;
    public decimal Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(); }
    }

    private decimal _netTotal;
    public decimal NetTotal
    {
        get => _netTotal;
        set { _netTotal = value; OnPropertyChanged(); }
    }

    // Commands
    public ICommand AddNewItemCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ConvertToInvoiceCommand { get; }
    public ICommand PrintCommand { get; }

    public PriceQuoteViewModel()
    {
        AddNewItemCommand = new RelayCommand(AddNewItem);
        SaveCommand = new RelayCommand(SaveQuote);
        ConvertToInvoiceCommand = new RelayCommand(ConvertToInvoice);
        PrintCommand = new RelayCommand(PrintQuote);

        LoadClients();
        if (Clients.Count == 0) LoadSampleClients(); // بيانات احتياطية
        LoadSampleQuoteItems(); // بيانات احتياطية

        QuoteItems.CollectionChanged += (s, e) => UpdateTotals();
    }

    // تحميل العملاء من الداتابيس
    private void LoadClients()
    {
        Clients.Clear();
        try
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT ClientID, Name FROM Clients ORDER BY Name COLLATE NOCASE", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Clients.Add(new Client
                {
                    ClientID = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
        }
        catch { /* لو فشل التحميل نتركها فارغة */ }
    }

    // بيانات احتياطية لو الداتابيس فاضية
    private void LoadSampleClients()
    {
        Clients.Add(new Client { ClientID = 1, Name = "عميل تجريبي 1" });
        Clients.Add(new Client { ClientID = 2, Name = "عميل تجريبي 2" });
        SelectedClient = Clients[0];
    }

    private void LoadSampleQuoteItems()
    {
        QuoteItems.Add(new QuoteItem { Name = "صنف تجريبي 1", Quantity = 2, UnitPrice = 100 });
        QuoteItems.Add(new QuoteItem { Name = "صنف تجريبي 2", Quantity = 1, UnitPrice = 250 });
    }

    public void UpdateTotals()
    {
        decimal total = 0;
        foreach (var item in QuoteItems)
            total += item.Total;

        Total = total;
        NetTotal = total - Discount;
    }

    private void AddNewItem()
    {
        QuoteItems.Add(new QuoteItem { Name = "", Quantity = 1, UnitPrice = 0 });
    }

    // حفظ عرض السعر في الداتابيس
    private void SaveQuote()
    {
        if (SelectedClient == null)
        {
            MessageBox.Show("يرجى اختيار العميل قبل الحفظ.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (QuoteItems.Count == 0)
        {
            MessageBox.Show("يرجى إضافة أصناف لعرض السعر.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();

            var insertQuoteCmd = new SQLiteCommand(
                @"INSERT INTO PriceQuotes (ClientID, QuoteDate, PaymentMethod, Discount, Total, NetTotal)
                  VALUES (@ClientID, @QuoteDate, @PaymentMethod, @Discount, @Total, @NetTotal);
                  SELECT last_insert_rowid();", conn);

            insertQuoteCmd.Parameters.AddWithValue("@ClientID", SelectedClient.ClientID);
            insertQuoteCmd.Parameters.AddWithValue("@QuoteDate", QuoteDate.ToString("yyyy-MM-dd"));
            insertQuoteCmd.Parameters.AddWithValue("@PaymentMethod", PaymentMethod);
            insertQuoteCmd.Parameters.AddWithValue("@Discount", Discount);
            insertQuoteCmd.Parameters.AddWithValue("@Total", Total);
            insertQuoteCmd.Parameters.AddWithValue("@NetTotal", NetTotal);

            long quoteId = (long)insertQuoteCmd.ExecuteScalar();

            foreach (var item in QuoteItems)
            {
                if (string.IsNullOrWhiteSpace(item.Name)) continue;

                var insertItemCmd = new SQLiteCommand(
                    @"INSERT INTO PriceQuoteItems (PriceQuoteID, ItemName, Quantity, UnitPrice)
                      VALUES (@PriceQuoteID, @ItemName, @Quantity, @UnitPrice)", conn);

                insertItemCmd.Parameters.AddWithValue("@PriceQuoteID", quoteId);
                insertItemCmd.Parameters.AddWithValue("@ItemName", item.Name);
                insertItemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                insertItemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);

                insertItemCmd.ExecuteNonQuery();
            }

            transaction.Commit();
            MessageBox.Show("تم حفظ عرض السعر بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء الحفظ: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // تحويل إلى فاتورة
    private void ConvertToInvoice()
    {
        if (SelectedClient == null)
        {
            MessageBox.Show("يرجى اختيار العميل أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (QuoteItems.Count == 0)
        {
            MessageBox.Show("لا يوجد أصناف للتحويل.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // منطق التحويل مشابه للحفظ لكن على جدول الفواتير
        // (يمكنك نسخه من SaveQuote مع تعديل الجدول)
    }

    private void PrintQuote()
    {
        // منطق الطباعة كما هو موجود في الكود الحالي
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
