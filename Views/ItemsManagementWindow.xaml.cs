using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            allItems = DatabaseHelper.GetAllItems() ?? new List<Item>();
            ApplyFilters();
        }

        private void LoadCategoryFilters()
        {
            var cat1List = allItems.Select(i => i.Cat1).Distinct().OrderBy(x => x).ToList();
            cat1List.Insert(0, "الكل");
            Category1Filter.ItemsSource = cat1List;
            Category1Filter.SelectedIndex = 0;

            var cat2List = allItems.Select(i => i.Cat2).Distinct().OrderBy(x => x).ToList();
            cat2List.Insert(0, "الكل");
            Category2Filter.ItemsSource = cat2List;
            Category2Filter.SelectedIndex = 0;

            var cat3List = allItems.Select(i => i.Cat3).Distinct().OrderBy(x => x).ToList();
            cat3List.Insert(0, "الكل");
            Category3Filter.ItemsSource = cat3List;
            Category3Filter.SelectedIndex = 0;

            var cat4List = allItems.Select(i => i.Cat4).Distinct().OrderBy(x => x).ToList();
            cat4List.Insert(0, "الكل");
            Category4Filter.ItemsSource = cat4List;
            Category4Filter.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filtered = allItems.AsEnumerable();

            string search = SearchTextBox.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.Where(i => i.ItemName.ToLower().Contains(search));

            if (Category1Filter.SelectedItem != null && Category1Filter.SelectedItem.ToString() != "الكل")
                filtered = filtered.Where(i => i.Cat1 == Category1Filter.SelectedItem.ToString());

            if (Category2Filter.SelectedItem != null && Category2Filter.SelectedItem.ToString() != "الكل")
                filtered = filtered.Where(i => i.Cat2 == Category2Filter.SelectedItem.ToString());

            if (Category3Filter.SelectedItem != null && Category3Filter.SelectedItem.ToString() != "الكل")
                filtered = filtered.Where(i => i.Cat3 == Category3Filter.SelectedItem.ToString());

            if (Category4Filter.SelectedItem != null && Category4Filter.SelectedItem.ToString() != "الكل")
                filtered = filtered.Where(i => i.Cat4 == Category4Filter.SelectedItem.ToString());

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
            var window = new AddItemWindow();
            if (window.ShowDialog() == true)
                LoadItems();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsDataGrid.SelectedItem is not Item selected) return;
            var window = new AddItemWindow(selected);
            window.ShowDialog();
            LoadItems();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsDataGrid.SelectedItem is not Item selected) return;
            if (MessageBox.Show($"هل تريد حذف الصنف {selected.ItemName}؟", "تأكيد", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseHelper.DeleteItem(selected.ItemID);
                LoadItems();
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadItems();
        }
    }
}