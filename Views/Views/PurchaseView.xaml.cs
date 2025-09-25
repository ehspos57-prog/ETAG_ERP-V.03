using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static ETAG_ERP.Models.Purchase;

namespace ETAG_ERP.Views
{
    public partial class PurchaseView : UserControl
    {
        private ObservableCollection<Invoice> purchases = new();
        private ObservableCollection<string> suppliers = new();
        private ObservableCollection<PurchaseItem> currentItems = new();

        public PurchaseView()
        {
            InitializeComponent();

            PurchasesGrid.ItemsSource = purchases;
            ItemsGrid.ItemsSource = currentItems;
            PurchaseDatePicker.SelectedDate = DateTime.Now;

            LoadSuppliers();
            LoadPurchases();
        }

        // تحميل الموردين (من الداتابيس أو مؤقت)
        private void LoadSuppliers()
        {
            suppliers.Clear();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                using var cmd = new SQLiteCommand("SELECT Name FROM Suppliers ORDER BY Name", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    suppliers.Add(reader.GetString(0));
            }
            catch
            {
                // بيانات احتياطية
                suppliers.Add("مورد 1");
                suppliers.Add("مورد 2");
                suppliers.Add("مورد 3");
            }

            SupplierComboBox.ItemsSource = suppliers;
            SupplierFilterComboBox.ItemsSource = suppliers;
        }

        // تحميل الفواتير
        private void LoadPurchases()
        {
            purchases.Clear();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                using var cmd = new SQLiteCommand("SELECT * FROM Purchases ORDER BY Date DESC", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    purchases.Add(new Invoice
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        InvoiceNumber = reader["InvoiceNumber"].ToString(),
                        SupplierName = reader["SupplierName"].ToString(),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        Description = reader["Description"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        Username = reader["Username"].ToString()
                    });
                }
            }
            catch
            {
                // بيانات احتياطية إذا فشل التحميل
                purchases.Add(new Invoice
                {
                    Id = 1,
                    InvoiceNumber = "P000001",
                    SupplierName = "مورد تجريبي",
                    Date = DateTime.Now,
                    Description = "فاتورة تجريبية",
                    TotalAmount = 1000,
                    Username = "Admin"
                });
            }

            PurchasesGrid.Items.Refresh();
        }

        // فتح نموذج إضافة فاتورة
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            FormPanel.Visibility = Visibility.Visible;
            PurchaseDatePicker.SelectedDate = DateTime.Now;
            SupplierComboBox.SelectedIndex = -1;
            DescriptionBox.Text = "";
            currentItems.Clear();
            UpdateTotalAmountText();
        }

        // إلغاء النموذج
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FormPanel.Visibility = Visibility.Collapsed;
        }

        // تحديث إجمالي العناصر
        private void UpdateTotalAmountText()
        {
            decimal total = (decimal)currentItems.Sum(i => i.Total);
            TotalAmountText.Text = total.ToString("N2");
        }

        // حفظ الفاتورة
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string supplier = SupplierComboBox.SelectedItem?.ToString();
            DateTime? date = PurchaseDatePicker.SelectedDate;
            string description = DescriptionBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(supplier) || date == null)
            {
                MessageBox.Show("يرجى إدخال اسم المورد والتاريخ.");
                return;
            }

            if (currentItems.Count == 0)
            {
                MessageBox.Show("يرجى إضافة أصناف للفاتورة.");
                return;
            }

            decimal total = (decimal)currentItems.Sum(i => i.Total);
            string invoiceNumber = "P" + (DateTime.Now.Ticks % 1000000).ToString("D6");

            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                using var transaction = conn.BeginTransaction();

                var cmd = new SQLiteCommand(
                    @"INSERT INTO Purchases (InvoiceNumber, SupplierName, Date, Description, TotalAmount, Username)
                      VALUES (@inv, @sup, @date, @desc, @total, @user); 
                      SELECT last_insert_rowid();", conn);
                cmd.Parameters.AddWithValue("@inv", invoiceNumber);
                cmd.Parameters.AddWithValue("@sup", supplier);
                cmd.Parameters.AddWithValue("@date", date.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@user", "Admin");

                long purchaseId = (long)cmd.ExecuteScalar();

                foreach (var item in currentItems)
                {
                    var itemCmd = new SQLiteCommand(
                        @"INSERT INTO PurchaseItems (PurchaseId, ItemCode, ItemName, Quantity, UnitPrice, Discount, Total)
                          VALUES (@pid, @code, @name, @qty, @price, @disc, @total)", conn);
                    itemCmd.Parameters.AddWithValue("@pid", purchaseId);
                    itemCmd.Parameters.AddWithValue("@code", item.ItemCode);
                    itemCmd.Parameters.AddWithValue("@name", item.ItemName);
                    itemCmd.Parameters.AddWithValue("@qty", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@price", item.UnitPrice);
                    itemCmd.Parameters.AddWithValue("@disc", item.Discount);
                    itemCmd.Parameters.AddWithValue("@total", item.Total);
                    itemCmd.ExecuteNonQuery();
                }

                transaction.Commit();
                MessageBox.Show("تم الحفظ بنجاح ✅");
                FormPanel.Visibility = Visibility.Collapsed;
                LoadPurchases();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الحفظ: {ex.Message}");
            }
        }

        // تحديث البيانات
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPurchases();
        }

        // فلترة
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string supplier = SupplierFilterComboBox.SelectedItem?.ToString() ?? "";
            DateTime? from = FromDatePicker.SelectedDate;
            DateTime? to = ToDatePicker.SelectedDate;
            string keyword = SearchTextBox.Text?.Trim() ?? "";

            var filtered = purchases.Where(p =>
                (string.IsNullOrEmpty(supplier) || p.SupplierName == supplier) &&
                (!from.HasValue || p.Date >= from.Value) &&
                (!to.HasValue || p.Date <= to.Value) &&
                (string.IsNullOrEmpty(keyword) || p.InvoiceNumber.Contains(keyword) || p.Description.Contains(keyword))
            ).ToList();

            PurchasesGrid.ItemsSource = filtered;
        }
    }
}
