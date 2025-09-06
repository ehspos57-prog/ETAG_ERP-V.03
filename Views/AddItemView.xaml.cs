using System;
using System.Windows;
using Microsoft.Win32;
using ETAG_ERP.Models;
using ETAG_ERP.Helpers;
using System.IO;

namespace ETAG_ERP.Views
{
    public partial class AddItemWindow : Window
    {
        public AddItemWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ✅ تحقق من البيانات
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("من فضلك أدخل اسم الصنف");
                    return;
                }

                if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice))
                {
                    MessageBox.Show("سعر البيع غير صالح");
                    return;
                }

                if (!decimal.TryParse(txtPurchasePrice.Text, out decimal purchasePrice))
                {
                    MessageBox.Show("سعر الشراء غير صالح");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int stockQty))
                {
                    MessageBox.Show("الكمية غير صالحة");
                    return;
                }

                int.TryParse(txtMinStock.Text, out int minStock);
                decimal.TryParse(txtTax.Text, out decimal tax);
                decimal.TryParse(txtDiscount.Text, out decimal discount);

                decimal? price1 = string.IsNullOrWhiteSpace(txtPrice1.Text) ? null : decimal.Parse(txtPrice1.Text);
                decimal? price2 = string.IsNullOrWhiteSpace(txtPrice2.Text) ? null : decimal.Parse(txtPrice2.Text);
                decimal? price3 = string.IsNullOrWhiteSpace(txtPrice3.Text) ? null : decimal.Parse(txtPrice3.Text);

                // ✅ إنشاء الصنف الجديد
                var newItem = new Item
                {
                    ItemName = txtName.Text.Trim(),
                    Code = txtCode.Text.Trim(),
                    Barcode = txtBarcode.Text.Trim(),
                    Unit = cmbUnit.Text,
                    StockQuantity = stockQty,
                    MinStock = minStock,
                    PurchasePrice = purchasePrice,
                    SellingPrice = sellingPrice,
                    Price1 = (decimal)price1,
                    Price2 = (decimal)price2,
                    Price3 = (decimal)price3,
                    Discount = discount,
                    Tax = tax,
                    IsActive = chkIsActive.IsChecked ?? true,
                    Description = txtDescription.Text.Trim(),
                    Category = GetSelectedCategory(),
                    ImagePath = txtImagePath.Text.Trim()
                };

                // ✅ حفظ في الداتابيس
                DatabaseHelper.InsertItem(newItem);

                MessageBox.Show("تم حفظ الصنف بنجاح ✅", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء الحفظ: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string? GetSelectedCategory()
        {
            if (!string.IsNullOrWhiteSpace(cmbCat5.Text)) return cmbCat5.Text;
            if (!string.IsNullOrWhiteSpace(cmbCat4.Text)) return cmbCat4.Text;
            if (!string.IsNullOrWhiteSpace(cmbCat3.Text)) return cmbCat3.Text;
            if (!string.IsNullOrWhiteSpace(cmbCat2.Text)) return cmbCat2.Text;
            if (!string.IsNullOrWhiteSpace(cmbCat1.Text)) return cmbCat1.Text;
            return null;
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "صور المنتجات|*.jpg;*.jpeg;*.png;*.gif|كل الملفات|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                txtImagePath.Text = dlg.FileName;
                ProductImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlg.FileName));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
