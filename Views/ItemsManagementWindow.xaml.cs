using System.Windows;

namespace ETAG_ERP.Views
{

    public partial class ItemsManagementWindow : Window
    {
        private List<Item> allItems = new();

        public ItemsManagementWindow()
        {
            InitializeComponent();
            LoadItems();
            LoadCategoryFilters();
        }

        private void LoadItems()
        {
            try
            {
                allItems = DatabaseHelper.GetAllItems() ?? new List<Item>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل الأصناف: {ex.Message}");
            }
        }

        private void LoadCategoryFilters()
        {
            var cat1List = allItems.Select(i => i.Cat1).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();
            cat1List.Insert(0, "الكل");
            Category1Filter.ItemsSource = cat1List;
            Category1Filter.SelectedIndex = 0;

            var cat2List = allItems.Select(i => i.Cat2).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();
            cat2List.Insert(0, "الكل");
            Category2Filter.ItemsSource = cat2List;
            Category2Filter.SelectedIndex = 0;

            var cat3List = allItems.Select(i => i.Cat3).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();
            cat3List.Insert(0, "الكل");
            Category3Filter.ItemsSource = cat3List;
            Category3Filter.SelectedIndex = 0;

            var cat4List = allItems.Select(i => i.Cat4).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();
            cat4List.Insert(0, "الكل");
            Category4Filter.ItemsSource = cat4List;
            Category4Filter.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filtered = allItems.AsEnumerable();

            string search = SearchTextBox.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.Where(i => i.ItemName?.ToLower().Contains(search) == true);

            if (Category1Filter.SelectedItem is string c1 && c1 != "الكل")
                filtered = filtered.Where(i => i.Cat1 == c1);

            if (Category2Filter.SelectedItem is string c2 && c2 != "الكل")
                filtered = filtered.Where(i => i.Cat2 == c2);

            if (Category3Filter.SelectedItem is string c3 && c3 != "الكل")
                filtered = filtered.Where(i => i.Cat3 == c3);

            if (Category4Filter.SelectedItem is string c4 && c4 != "الكل")
                filtered = filtered.Where(i => i.Cat4 == c4);

            ItemsDataGrid.ItemsSource = filtered.ToList();
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddItemWindow();
            if (addWindow.ShowDialog() == true)
                LoadItems();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsDataGrid.SelectedItem is not Item selected)
            {
                MessageBox.Show("الرجاء اختيار صنف لتعديله.");
                return;
            }

            var editWindow = new AddItemWindow(selected);
            if (editWindow.ShowDialog() == true)
                LoadItems();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsDataGrid.SelectedItem is not Item selected) return;

            // حذف مباشر من القاعدة (صامت) ثم تحديث الواجهة فورياً
            bool deleted = DatabaseHelper.DeleteItem(selected.Id);
            if (deleted)
            {
                allItems.Remove(selected);
                ApplyFilters(); // يعيد توجيه المصدر ويحدث الجدول فوراً
            }
            else
            {
                // في حال أردت إعلام المستخدم بفشل الحذف فعلياً يمكنك إظهار رسالة هنا
                // MessageBox.Show("فشل الحذف. تحقق من قاعدة البيانات.");
            }
        }



        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadItems();
        }


    }
}
