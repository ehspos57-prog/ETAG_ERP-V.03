using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class CategoryView : UserControl
    {
        private List<Item> allItems = new();       // كل الأصناف
        private List<Item> filteredItems = new();  // للأصناف بعد البحث
        private List<string> categories = new();   // التصنيفات
        private int currentCategoryIndex = 0;

        public CategoryView()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                allItems = DatabaseHelper.GetAllItems() ?? new List<Item>();

                // fallback لو فاضي
                if (allItems.Count == 0)
                {
                    allItems = new List<Item>
                    {
                        new Item { ItemName = "صنف 1 (تجريبي)", ImagePath = "Images/item1.png", Category = "افتراضي" },
                        new Item { ItemName = "صنف 2 (تجريبي)", ImagePath = "Images/item2.png", Category = "افتراضي" },
                        new Item { ItemName = "صنف 3 (تجريبي)", ImagePath = "Images/item3.png", Category = "افتراضي" }
                    };
                }

                // التصنيفات
                categories = allItems
                    .Where(i => !string.IsNullOrWhiteSpace(i.Category))
                    .Select(i => i.Category)
                    .Distinct()
                    .ToList();

                // دايمًا "الكل" يبقى أول عنصر
                categories.Insert(0, "الكل");

                currentCategoryIndex = 0;
                filteredItems = allItems;
                ShowCurrentCategory();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل الأصناف: {ex.Message}");
            }
        }

        private void ShowCurrentCategory()
        {
            if (categories.Count == 0)
            {
                CategoryCardsPanel.ItemsSource = null;
                return;
            }

            string currentCategory = categories[currentCategoryIndex];
            CategoryTitle.Text = $"التصنيف الحالي: {currentCategory}";

            var itemsToShow = currentCategory == "الكل"
                ? filteredItems
                : filteredItems.Where(i => i.Category == currentCategory).ToList();

            CategoryCardsPanel.ItemsSource = itemsToShow.Count > 0
                ? itemsToShow
                : new List<Item> { new Item { ItemName = "لا توجد أصناف في هذا التصنيف" } };
        }

        private void ProductSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ProductSearchTextBox.Text.ToLower();

            filteredItems = string.IsNullOrWhiteSpace(searchText)
                ? allItems
                : allItems.Where(i => i.ItemName.ToLower().Contains(searchText)).ToList();

            ShowCurrentCategory();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddItemWindow();
            addWindow.ShowDialog();
            LoadCategories();
        }

        private void BackCategory_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategoryIndex > 0)
            {
                currentCategoryIndex--;
                ShowCurrentCategory();
            }
        }

        private void NextCategory_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategoryIndex < categories.Count - 1)
            {
                currentCategoryIndex++;
                ShowCurrentCategory();
            }
        }
    }
}
