using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class AddItemWindow : Window
    {
        private List<CategorySeedData> _categories = new List<CategorySeedData>
        {
            new CategorySeedData("Water", "Avis", "Stainless", "", "", "", "WASTA"),
            new CategorySeedData("Water", "Avis", "Steel", "", "", "", "WASTE"),
            new CategorySeedData("Water", "Flange", "back up ring", "", "", "", "WFLBR"),
            new CategorySeedData("Water", "Flange", "Flange Blind", "", "", "", "WFLFB"),
            new CategorySeedData("Water", "Flange", "pump flange", "", "", "", "WFLPF"),
            new CategorySeedData("Water", "Flange", "Flange Slip", "", "", "", "WFLFS"),
            new CategorySeedData("Water", "Flange", "socket weld", "", "", "", "WFLSW"),
            new CategorySeedData("Water", "Valve", "butterfly valve", "", "", "", "WVBV"),
            new CategorySeedData("Water", "Valve", "carbon steel ball valve", "", "", "", "WVCS"),
            new CategorySeedData("Water", "Valve", "check valve", "", "", "", "WVCV"),
            new CategorySeedData("Water", "Valve", "gear box", "", "", "", "WVGB"),
            new CategorySeedData("Water", "Valve", "globe valve", "", "", "", "WVGV"),
            new CategorySeedData("Water", "Valve", "stainless steel ball valve", "", "", "", "WVSSBV"),
            new CategorySeedData("Water", "Fitting", "CAM LOCK AL", "", "", "", "WFCLA"),
            new CategorySeedData("Water", "Fitting", "CAM LOCK CAST", "", "", "", "WFCLC"),
            new CategorySeedData("Water", "Fitting", "CAM LOCK ST", "", "", "", "WFCLST"),
            new CategorySeedData("Water", "Fitting", "cast iron", "", "", "", "WFCI"),
            new CategorySeedData("pneumatic", "cylinder", "", "", "", "", "PNCY"),
            new CategorySeedData("pneumatic", "FRL", "", "", "", "", "PNFRL"),
            new CategorySeedData("pneumatic", "golbe valve", "", "", "", "", "PNGV"),
            new CategorySeedData("pneumatic", "HOSE", "", "", "", "", "PNHO"),
            new CategorySeedData("pneumatic", "VALVE", "", "", "", "", "PNVA"),
            new CategorySeedData("pneumatic", "metal", "el Bow", "", "", "", "PNMEB"),
            new CategorySeedData("pneumatic", "metal", "nipple", "", "", "", "PNMN"),
            new CategorySeedData("pneumatic", "metal", "straight", "", "", "", "PNMS"),
            new CategorySeedData("pneumatic", "metal", "TEE", "", "", "", "PNMTE"),
            new CategorySeedData("pneumatic", "Plastic", "bushing", "", "", "", "PNPB"),
            new CategorySeedData("pneumatic", "Plastic", "CROSS", "", "", "", "PNPC"),
            new CategorySeedData("pneumatic", "Plastic", "el Bow special", "", "", "", "PNPEBS"),
            new CategorySeedData("pneumatic", "Plastic", "Panel", "", "", "", "PNPA"),
            new CategorySeedData("pneumatic", "Plastic", "PLUG", "", "", "", "PNPL"),
            new CategorySeedData("pneumatic", "Plastic", "silensor", "", "", "", "PNPSI"),
            new CategorySeedData("pneumatic", "Plastic", "straight", "", "", "", "PNPST"),
            new CategorySeedData("pneumatic", "Plastic", "TEE", "", "", "", "PNPTE"),
            new CategorySeedData("pneumatic", "Plastic", "Throttle&check", "", "", "", "PNPTH"),
            new CategorySeedData("pneumatic", "Plastic", "y", "", "", "", "PNPY"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "ADAPTOR", "", "", "HFGAD"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "BUSH", "", "", "HFGBU"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "BUSHING", "", "", "HFGBS"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "CROSS", "", "", "HFGC"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "EL BOW", "", "", "HFGEB"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "EL BOW JIC", "", "", "HFGEBJ"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "NIPPLE", "", "", "HFGN"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "PLUG", "", "", "HFGPL"),
            new CategorySeedData("hydrulic", "fitting", "GERMAN", "TEE", "", "", "HFGTE"),
            new CategorySeedData("hydrulic", "accessories", "Air breather cap", "", "", "", "HAABC"),
            new CategorySeedData("hydrulic", "accessories", "flange", "", "", "", "HAFL"),
            new CategorySeedData("hydrulic", "accessories", "SPLIT FLANGE", "", "", "", "HASPFL"),
            new CategorySeedData("hydrulic", "accessories", "TEST HOSE", "", "", "", "HATH"),
            new CategorySeedData("hydrulic", "accessories", "TEST POINT", "", "", "", "HATP"),
            new CategorySeedData("hydrulic", "accessories", "VISUAL LEVEL", "", "", "", "HAVL"),
            new CategorySeedData("hydrulic", "filter", "FILTER ELEMENT", "", "", "", "HFFE"),
            new CategorySeedData("hydrulic", "filter", "FILTER", "", "", "", "HFFIL"),
            new CategorySeedData("hydrulic", "GAUGE", "Pressure gauge", "with glycerine", "", "", "HGPGWG"),
            new CategorySeedData("hydrulic", "GAUGE", "Pressure gauge", "with out glycerine", "", "", "HGPGWOG"),
            new CategorySeedData("hydrulic", "GAUGE", "temprture gauge", "", "", "", "HGTEG"),
            new CategorySeedData("hydrulic", "pump&motor", "motor", "", "", "", "HPMM"),
            new CategorySeedData("hydrulic", "pump&motor", "power steering", "", "", "", "HPMPS"),
            new CategorySeedData("hydrulic", "pump&motor", "pump", "", "", "", "HPMPU"),
            new CategorySeedData("hydrulic", "valve", "coil", "", "", "", "HVCOI"),
            new CategorySeedData("hydrulic", "valve", "control", "", "", "", "HVCON"),
            new CategorySeedData("hydrulic", "valve", "modular valve", "", "", "", "HVMV"),
            new CategorySeedData("hydrulic", "valve", "in line valve", "", "", "", "HVILV"),
            new CategorySeedData("SEAL", "dust seal", "METAL CASE", "", "", "", "SDSMC"),
            new CategorySeedData("SEAL", "dust seal", "NPR", "", "", "", "SDSNPR"),
            new CategorySeedData("SEAL", "hydraulic seal", "", "", "", "SHS"),
            new CategorySeedData("SEAL", "KGD", "", "", "", "", "SKGD"),
            new CategorySeedData("SEAL", "Mechanicul seal", "CONICAL", "", "", "", "SMESC"),
            new CategorySeedData("SEAL", "Mechanicul seal", "STRAIGHT", "", "", "", "SMESS"),
            new CategorySeedData("SEAL", "MPS", "", "", "", "", "SMPS"),
            new CategorySeedData("SEAL", "OMEGA", "KOMATSU", "", "", "", "SOMKO"),
            new CategorySeedData("SEAL", "OMEGA", "PISTON SEAL", "", "", "", "SOMPS"),
            new CategorySeedData("SEAL", "OMEGA", "ROD SEAL", "", "", "", "SOMRS"),
            new CategorySeedData("SEAL", "PACICING RING", "", "", "", "", "SPAR"),
            new CategorySeedData("SEAL", "pneumatic seal", "E4", "", "", "", "SPNSE4"),
            new CategorySeedData("SEAL", "pneumatic seal", "EU", "", "", "", "SPNSEU"),
            new CategorySeedData("SEAL", "pneumatic seal", "PP", "", "", "", "SPNSPP"),
            new CategorySeedData("SEAL", "RUBBER COUPLING", "", "", "", "", "SRUCO"),
            new CategorySeedData("SEAL", "shaft seal", "METAL CASE", "", "", "", "SSSMC"),
            new CategorySeedData("SEAL", "shaft seal", "NBR", "", "", "", "SSSNBR"),
            new CategorySeedData("SEAL", "x RING", "", "", "", "", "SXRNG"),
            new CategorySeedData("SEAL", "Oring", "VITON", "", "", "", "SORVI"),
            new CategorySeedData("SEAL", "Oring", "Silicone", "", "", "", "SORSI"),
            new CategorySeedData("SEAL", "Oring", "Teflon", "", "", "", "SORTE"),
            new CategorySeedData("SEAL", "Oring", "ARTELON", "", "", "", "SORAR"),
            new CategorySeedData("SEAL", "Oring", "oring rope", "", "", "", "SORORO"),
            new CategorySeedData("SEAL", "Oring", "ORING BOX", "", "", "", "SORORB"),
            new CategorySeedData("SEAL", "Oring", "NBR", "1", "", "", "SORNBR1"),
            new CategorySeedData("SEAL", "Oring", "NBR", "1.5", "", "", "SORNBR2"),
            new CategorySeedData("SEAL", "Oring", "NBR", "2.5", "", "", "SORNBR3"),
            new CategorySeedData("SEAL", "Oring", "NBR", "1.6", "", "", "SORNBR4"),
            new CategorySeedData("SEAL", "Oring", "NBR", "1.78", "", "", "SORNBR5"),
            new CategorySeedData("SEAL", "Oring", "NBR", "2", "", "", "SORNBR6"),
            new CategorySeedData("SEAL", "Oring", "NBR", "2.4", "", "", "SORNBR7"),
            new CategorySeedData("SEAL", "Oring", "NBR", "2.5", "", "", "SORNBR8"),
            new CategorySeedData("SEAL", "Oring", "NBR", "2.62", "", "", "SORNBR9"),
            new CategorySeedData("SEAL", "Oring", "NBR", "3", "", "", "SORNB3"),
            new CategorySeedData("SEAL", "Oring", "NBR", "3.5", "", "", "SORNB4"),
            new CategorySeedData("SEAL", "Oring", "NBR", "3.53", "", "", "SORNB5"),
            new CategorySeedData("SEAL", "Oring", "NBR", "4", "", "", "SORNB6"),
            new CategorySeedData("SEAL", "Oring", "NBR", "4.5", "", "", "SORNB7"),
            new CategorySeedData("SEAL", "Oring", "NBR", "5", "", "", "SORNB8"),
            new CategorySeedData("SEAL", "Oring", "NBR", "5.7", "", "", "SORNB9"),
            new CategorySeedData("SEAL", "Oring", "NBR", "5.33", "", "", "SORINBR5"),
            new CategorySeedData("SEAL", "Oring", "NBR", "6", "", "", "SORINBR1"),
            new CategorySeedData("SEAL", "Oring", "NBR", "6.5", "", "", "SORINBR2"),
            new CategorySeedData("SEAL", "Oring", "NBR", "6.99", "", "", "SORINBR3"),
            new CategorySeedData("SEAL", "Oring", "NBR", "7", "", "", "SORINBR4"),
            new CategorySeedData("SEAL", "Oring", "NBR", "8", "", "", "SORINBR6"),
            new CategorySeedData("SEAL", "Oring", "NBR", "1.2", "", "", "SORINBR7"),
            new CategorySeedData("SEAL", "Oring", "NBR", "1.8", "", "", "SORINBR8"),
            new CategorySeedData("SEAL", "Oring", "NBR", "2.3", "", "", "SORINBR9"),
            new CategorySeedData("SEAL", "Oring", "NBR", "3.2", "", "", "SONBRS3"),
            new CategorySeedData("SEAL", "D RING", "", "", "", "", "SDRNG")
        };

        public AddItemWindow()
        {
            InitializeComponent();
            LoadCategoriesLevel(cmbCat1, null);
            Loaded += AddItemWindow_Loaded;
        }

        private void LoadCategoriesLevel(ComboBox combo, int? parentId)
        {
            var dt = DatabaseHelper.GetCategories(parentId);
            combo.ItemsSource = dt.DefaultView;
            combo.DisplayMemberPath = "Name";
            combo.SelectedValuePath = "Id";
        }


        private void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategoriesLevel1();
        }

        /// <summary>
        /// Load level 1 categories from both Seed and DB
        /// </summary>
        private void LoadCategoriesLevel1()
        {
            var dbCategories = new List<string>();
            var dt = DatabaseHelper.ExecuteQuery("SELECT Name FROM Categories WHERE ParentId IS NULL") as DataTable;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                    dbCategories.Add(row["Name"].ToString());
            }

            var seedCategories = _categories
                .Select(c => c.Level1)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .ToList();

            var merged = seedCategories
                .Union(dbCategories)
                .OrderBy(x => x)
                .ToList();

            cmbCat1.ItemsSource = merged;
        }

        private void cmbCat1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cmbCat1.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected)) return;

            // From DB
            var dbCategories = new List<string>();
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Name FROM Categories WHERE ParentId IN (SELECT Id FROM Categories WHERE Name=@Name)",
                new Dictionary<string, object> { { "@Name", selected } }) as DataTable;

            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    dbCategories.Add(row["Name"].ToString());

            // From Seed
            var seedCategories = _categories
                .Where(c => c.Level1 == selected && !string.IsNullOrWhiteSpace(c.Level2))
                .Select(c => c.Level2)
                .Distinct()
                .ToList();

            cmbCat2.ItemsSource = seedCategories.Union(dbCategories).OrderBy(x => x).ToList();
        }

        private void cmbCat2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected1 = cmbCat1.SelectedItem?.ToString();
            var selected2 = cmbCat2.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected2)) return;

            var dbCategories = new List<string>();
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Name FROM Categories WHERE ParentId IN (SELECT Id FROM Categories WHERE Name=@Name)",
                new Dictionary<string, object> { { "@Name", selected2 } }) as DataTable;

            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    dbCategories.Add(row["Name"].ToString());

            var seedCategories = _categories
                .Where(c => c.Level1 == selected1 && c.Level2 == selected2 && !string.IsNullOrWhiteSpace(c.Level3))
                .Select(c => c.Level3)
                .Distinct()
                .ToList();

            cmbCat3.ItemsSource = seedCategories.Union(dbCategories).OrderBy(x => x).ToList();
        }

        private void cmbCat3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected1 = cmbCat1.SelectedItem?.ToString();
            var selected2 = cmbCat2.SelectedItem?.ToString();
            var selected3 = cmbCat3.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected3)) return;

            var dbCategories = new List<string>();
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Name FROM Categories WHERE ParentId IN (SELECT Id FROM Categories WHERE Name=@Name)",
                new Dictionary<string, object> { { "@Name", selected3 } }) as DataTable;

            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    dbCategories.Add(row["Name"].ToString());

            var seedCategories = _categories
                .Where(c => c.Level1 == selected1 && c.Level2 == selected2 && c.Level3 == selected3 && !string.IsNullOrWhiteSpace(c.Level4))
                .Select(c => c.Level4)
                .Distinct()
                .ToList();

            cmbCat4.ItemsSource = seedCategories.Union(dbCategories).OrderBy(x => x).ToList();
        }

        private void cmbCat4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected1 = cmbCat1.SelectedItem?.ToString();
            var selected2 = cmbCat2.SelectedItem?.ToString();
            var selected3 = cmbCat3.SelectedItem?.ToString();
            var selected4 = cmbCat4.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected4)) return;

            var dbCategories = new List<string>();
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Name FROM Categories WHERE ParentId IN (SELECT Id FROM Categories WHERE Name=@Name)",
                new Dictionary<string, object> { { "@Name", selected4 } }) as DataTable;

            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    dbCategories.Add(row["Name"].ToString());

            var seedCategories = _categories
                .Where(c => c.Level1 == selected1 && c.Level2 == selected2 && c.Level3 == selected3 && c.Level4 == selected4 && !string.IsNullOrWhiteSpace(c.Level5))
                .Select(c => c.Level5)
                .Distinct()
                .ToList();

            cmbCat5.ItemsSource = seedCategories.Union(dbCategories).OrderBy(x => x).ToList();
        }

        private void cmbCat5_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private string? SelectedCategory
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(cmbCat5.Text)) return cmbCat5.Text;
                if (!string.IsNullOrWhiteSpace(cmbCat4.Text)) return cmbCat4.Text;
                if (!string.IsNullOrWhiteSpace(cmbCat3.Text)) return cmbCat3.Text;
                if (!string.IsNullOrWhiteSpace(cmbCat2.Text)) return cmbCat2.Text;
                if (!string.IsNullOrWhiteSpace(cmbCat1.Text)) return cmbCat1.Text;
                return null;
            }
        }
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
                    MessageBox.Show("سعر البيع غير صالح"); return;
                }
                if (!decimal.TryParse(txtPurchasePrice.Text, out decimal purchasePrice))
                {
                    MessageBox.Show("سعر الشراء غير صالح"); return;
                }
                if (!int.TryParse(txtQuantity.Text, out int stockQty))
                {
                    MessageBox.Show("الكمية غير صالحة"); return;
                }

                int.TryParse(txtMinStock.Text, out int minStock);
                decimal.TryParse(txtTax.Text, out decimal tax);
                decimal.TryParse(txtDiscount.Text, out decimal discount);
                decimal? price1 = string.IsNullOrWhiteSpace(txtPrice1.Text) ? null : decimal.Parse(txtPrice1.Text);
                decimal? price2 = string.IsNullOrWhiteSpace(txtPrice2.Text) ? null : decimal.Parse(txtPrice2.Text);
                decimal? price3 = string.IsNullOrWhiteSpace(txtPrice3.Text) ? null : decimal.Parse(txtPrice3.Text);

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
                    Price1 = price1 ?? 0,
                    Price2 = price2 ?? 0,
                    Price3 = price3 ?? 0,
                    Discount = discount,
                    Tax = tax,
                    IsActive = chkIsActive.IsChecked ?? true,
                    Description = txtDescription.Text.Trim(),
                    Category = SelectedCategory,
                    ImagePath = txtImagePath.Text.Trim()
                };

                DatabaseHelper.InsertItem(newItem);
                MessageBox.Show("تم حفظ الصنف بنجاح ✅", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
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
            var dlg = new OpenFileDialog { Filter = "صور المنتجات|*.jpg;*.jpeg;*.png;*.gif|كل الملفات|*.*" };
            if (dlg.ShowDialog() == true)
            {
                txtImagePath.Text = dlg.FileName;
                ProductImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlg.FileName));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();


    }
}
