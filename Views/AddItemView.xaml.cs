using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class AddItemWindow : Window
    {
        private List<ETAG_ERP.Helpers.CategorySeedData> _categories;
        private Item item;
        private bool isEditMode = false;

        // Constructor للصنف الجديد
        public AddItemWindow()
        {
            InitializeComponent();

            // ✅ تأكد أن السيدنج تم (في أول تشغيل فقط)
            DatabaseHelper.SeedCategoriesIfEmpty();

            // ✅ تحميل التصنيفات من القاعدة
            var dbCategories = DatabaseHelper.GetAllCategories();
            _categories = dbCategories.Select(c => new ETAG_ERP.Helpers.CategorySeedData(
                c.Level1, c.Level2, c.Level3, c.Level4, c.Level5, c.Code
            )).ToList();

            LoadCategoriesLevel1();
            this.item = null;
        }

        // Constructor لتعديل صنف موجود

        public AddItemWindow(Item existingItem)
        {
            InitializeComponent();

            // ✅ نفس الإجراء عند التعديل
            DatabaseHelper.SeedCategoriesIfEmpty();

            var dbCategories = DatabaseHelper.GetAllCategories();
            _categories = dbCategories.Select(c => new ETAG_ERP.Helpers.CategorySeedData(
                c.Level1, c.Level2, c.Level3, c.Level4, c.Level5, c.Code
            )).ToList();

            LoadCategoriesLevel1();

            this.item = existingItem;
            this.isEditMode = true;
            LoadItemData();
        }

        // تحميل بيانات الصنف في حالة التعديل
        private void LoadItemData()
        {
            if (item == null) return;

            txtName.Text = item.ItemName;
            txtCode.Text = item.Code;
            txtBarcode.Text = item.Barcode;
            cmbUnit.Text = item.Unit;
            txtQuantity.Text = item.Quantity.ToString();
            txtMinStock.Text = item.MinStock.ToString();
            txtPurchasePrice.Text = item.PurchasePrice.ToString();
            txtSellingPrice.Text = item.SellingPrice.ToString();
            txtPrice1.Text = item.Price1.ToString();
            txtPrice2.Text = item.Price2.ToString();
            txtPrice3.Text = item.Price3.ToString();
            txtDiscount.Text = item.Discount.ToString();
            txtTax.Text = item.Tax.ToString();
            chkIsActive.IsChecked = item.IsActive;
            txtDescription.Text = item.Description;
            txtImagePath.Text = item.ImagePath;

            if (!string.IsNullOrEmpty(item.ImagePath) && System.IO.File.Exists(item.ImagePath))
            {
                ProductImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(item.ImagePath));
            }

            cmbCat1.SelectedItem = item.Cat1;
            cmbCat2.SelectedItem = item.Cat2;
            cmbCat3.SelectedItem = item.Cat3;
            cmbCat4.SelectedItem = item.Cat4;
            cmbCat5.SelectedItem = item.Cat5;
        }

        // تحميل المستوى الأول من التصنيفات
        private void LoadCategoriesLevel1()
        {
            try
            {
                var level1 = _categories
                    .Select(c => c.Level1)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                cmbCat1.ItemsSource = level1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تحميل المستوى الأول من التصنيفات: " + ex.Message);
            }
        }

        private void cmbCat1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected1 = cmbCat1.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected1)) return;

            var level2 = _categories
                .Where(c => c.Level1 == selected1 && !string.IsNullOrWhiteSpace(c.Level2))
                .Select(c => c.Level2)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbCat2.ItemsSource = level2;
            cmbCat3.ItemsSource = null;
            cmbCat4.ItemsSource = null;
            cmbCat5.ItemsSource = null;
        }

        private void cmbCat2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected1 = cmbCat1.SelectedItem?.ToString();
            var selected2 = cmbCat2.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected2)) return;

            var level3 = _categories
                .Where(c => c.Level1 == selected1 && c.Level2 == selected2 && !string.IsNullOrWhiteSpace(c.Level3))
                .Select(c => c.Level3)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbCat3.ItemsSource = level3;
            cmbCat4.ItemsSource = null;
            cmbCat5.ItemsSource = null;
        }

        private void cmbCat3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected1 = cmbCat1.SelectedItem?.ToString();
            var selected2 = cmbCat2.SelectedItem?.ToString();
            var selected3 = cmbCat3.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected3)) return;

            var level4 = _categories
                .Where(c => c.Level1 == selected1 && c.Level2 == selected2 && c.Level3 == selected3 && !string.IsNullOrWhiteSpace(c.Level4))
                .Select(c => c.Level4)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbCat4.ItemsSource = level4;
            cmbCat5.ItemsSource = null;
        }

        private void cmbCat4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected1 = cmbCat1.SelectedItem?.ToString();
            var selected2 = cmbCat2.SelectedItem?.ToString();
            var selected3 = cmbCat3.SelectedItem?.ToString();
            var selected4 = cmbCat4.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected4)) return;

            var level5 = _categories
                .Where(c => c.Level1 == selected1 && c.Level2 == selected2 && c.Level3 == selected3 && c.Level4 == selected4 && !string.IsNullOrWhiteSpace(c.Level5))
                .Select(c => c.Level5)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbCat5.ItemsSource = level5;
        }

        // دالة حفظ الصنف
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
                decimal.TryParse(txtPrice1.Text, out decimal p1);
                decimal.TryParse(txtPrice2.Text, out decimal p2);
                decimal.TryParse(txtPrice3.Text, out decimal p3);

                var (cat1, cat2, cat3, cat4, cat5) = GetCategoryValues();

                var newItem = new Item
                {
                    ItemName = txtName.Text.Trim(),
                    Code = txtCode.Text.Trim(),
                    Barcode = txtBarcode.Text.Trim(),
                    Unit = cmbUnit.Text,
                    Quantity = stockQty,
                    MinStock = minStock,
                    PurchasePrice = purchasePrice,
                    SellingPrice = sellingPrice,
                    Price1 = p1,
                    Price2 = p2,
                    Price3 = p3,
                    Discount = discount,
                    Tax = tax,
                    IsActive = chkIsActive.IsChecked ?? true,
                    Description = txtDescription.Text.Trim(),
                    Cat1 = cat1,
                    Cat2 = cat2,
                    Cat3 = cat3,
                    Cat4 = cat4,
                    Cat5 = cat5,
                    ImagePath = txtImagePath.Text.Trim()
                };

                bool success;

                if (isEditMode && item != null)
                {
                    newItem.ItemID = item.ItemID;
                    success = DatabaseHelper.UpdateItem(newItem);
                }
                else
                {
                    success = DatabaseHelper.InsertItem(newItem);
                }

                if (success)
                {
                    MessageBox.Show(isEditMode ? "تم تحديث بيانات الصنف بنجاح ✅" : "تم حفظ الصنف بنجاح ✅", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("فشل في حفظ البيانات. تحقق من قاعدة البيانات.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء الحفظ: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private (string cat1, string cat2, string cat3, string cat4, string cat5) GetCategoryValues()
        {
            var c1 = cmbCat1.SelectedItem?.ToString();
            var c2 = cmbCat2.SelectedItem?.ToString();
            var c3 = cmbCat3.SelectedItem?.ToString();
            var c4 = cmbCat4.SelectedItem?.ToString();
            var c5 = cmbCat5.SelectedItem?.ToString();
            return (c1, c2, c3, c4, c5);
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "صور المنتجات|*.jpg;*.jpeg;*.png;*.gif|كل الملفات|*.*" };
            if (dlg.ShowDialog() == true)
            {
                txtImagePath.Text = dlg.FileName;
                ProductImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlg.FileName));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
