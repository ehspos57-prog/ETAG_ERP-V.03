using ETAG_ERP.Helpers;
using System.Windows;
using System.Windows.Controls;
using ETAG_ERP.Models;


namespace ETAG_ERP.Views
{
    public partial class CategoryView : UserControl
    {
        private List<Item> allItems = new();       // كل الأصناف
        private List<Item> filteredItems = new();  // للأصناف بعد البحث
        private List<ETAG_ERP.Helpers.CategorySeedData> categorySeed = new();
        // السييد الكامل

        private int _currentLevel = 1;
        private object _selectedLevel1;
        private object _selectedLevel2;
        private object _selectedLevel3;
        private object _selectedLevel4;

        private int _currentFamilyIndex = 0;
        private List<string> _currentFamilies = new List<string>();

        public CategoryView()
        {
            InitializeComponent();
            LoadCategories();
        }

        #region 🔹 تحميل البيانات

        private void LoadCategories()
        {
            try
            {
                allItems = DatabaseHelper.GetAllItems() ?? new List<Item>();
                categorySeed = ETAG_ERP.Helpers.CategorySeeder.GetSeedData();

                ResetSelection();
                ShowCurrentLevel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل الأصناف والتصنيفات: {ex.Message}");
            }
        }

        #endregion

        #region 🔹 عرض المستويات الهرمية

        private void ShowCurrentLevel()
        {
            switch (_currentLevel)
            {
                case 1:
                    DisplayLevel1();
                    break;
                case 2:
                    LoadLevel(_selectedLevel1, null, null, null, 2);
                    break;
                case 3:
                    LoadLevel(_selectedLevel1, _selectedLevel2, null, null, 3);
                    break;
                case 4:
                    LoadLevel(_selectedLevel1, _selectedLevel2, _selectedLevel3, null, 4);
                    break;
                case 5:
                    LoadLevel(_selectedLevel1, _selectedLevel2, _selectedLevel3, _selectedLevel4, 5);
                    break;
            }
        }

        private void DisplayLevel1()
        {
            _currentFamilies = categorySeed
                .Where(s => !string.IsNullOrWhiteSpace(s.Level1))
                .Select(s => s.Level1)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            CategoryTitle.Text = "الفاميليز";
            CategoryCardsPanel.ItemsSource = _currentFamilies.Select(f => new Item { ItemName = f }).ToList();
        }

        private void LoadLevel(object cat1, object cat2, object cat3, object cat4, int level)
        {
            IEnumerable<string> nextLevels = Enumerable.Empty<string>();
            string title = "";

            switch (level)
            {
                case 2:
                    nextLevels = categorySeed
                        .Where(s => s.Level1 == (string)cat1 && !string.IsNullOrWhiteSpace(s.Level2))
                        .Select(s => s.Level2)
                        .Distinct();
                    title = (string)cat1;
                    break;
                case 3:
                    nextLevels = categorySeed
                        .Where(s => s.Level1 == (string)cat1 && s.Level2 == (string)cat2 && !string.IsNullOrWhiteSpace(s.Level3))
                        .Select(s => s.Level3)
                        .Distinct();
                    title = $"{cat1} / {cat2}";
                    break;
                case 4:
                    nextLevels = categorySeed
                        .Where(s => s.Level1 == (string)cat1 && s.Level2 == (string)cat2 && s.Level3 == (string)cat3 && !string.IsNullOrWhiteSpace(s.Level4))
                        .Select(s => s.Level4)
                        .Distinct();
                    title = $"{cat1} / {cat2} / {cat3}";
                    break;
                case 5:
                    var items = allItems
                        .Where(i =>
                            i.Cat1 == (string)cat1 &&
                            i.Cat2 == (string)cat2 &&
                            i.Cat3 == (string)cat3 &&
                            i.Cat4 == (string)cat4)
                        .ToList();

                    CategoryTitle.Text = $"{cat1} / {cat2} / {cat3} / {cat4}";
                    CategoryCardsPanel.ItemsSource = items.Count > 0 ? items : new List<Item> { new Item { ItemName = "لا توجد أصناف" } };
                    return;
            }

            var list = nextLevels.OrderBy(x => x).Select(x => new Item { ItemName = x }).ToList();
            CategoryTitle.Text = title;
            CategoryCardsPanel.ItemsSource = list;

            // الانتقال التلقائي للمستوى التالي إذا عنصر واحد فقط
            if (list.Count == 1)
            {
                switch (level)
                {
                    case 2: _selectedLevel2 = list[0].ItemName; _currentLevel = 3; LoadLevel(cat1, _selectedLevel2, null, null, 3); break;
                    case 3: _selectedLevel3 = list[0].ItemName; _currentLevel = 4; LoadLevel(cat1, cat2, _selectedLevel3, null, 4); break;
                    case 4: _selectedLevel4 = list[0].ItemName; _currentLevel = 5; LoadLevel(cat1, cat2, cat3, _selectedLevel4, 5); break;
                }
            }
        }

        #endregion

        #region 🔹 واجهة المستخدم

        private void ItemCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not Border border || border.DataContext is not Item selected) return;

            switch (_currentLevel)
            {
                case 1: _selectedLevel1 = selected.ItemName; _currentLevel = 2; ShowCurrentLevel(); break;
                case 2: _selectedLevel2 = selected.ItemName; _currentLevel = 3; ShowCurrentLevel(); break;
                case 3: _selectedLevel3 = selected.ItemName; _currentLevel = 4; ShowCurrentLevel(); break;
                case 4: _selectedLevel4 = selected.ItemName; _currentLevel = 5; ShowCurrentLevel(); break;
                case 5:
                    new AddItemWindow(selected).ShowDialog();
                    ReloadAfterEditOrAdd();
                    break;
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (new AddItemWindow().ShowDialog() == true)
            {
                ReloadAfterEditOrAdd();
            }
        }

        private void ReloadAfterEditOrAdd()
        {
            allItems = DatabaseHelper.GetAllItems() ?? new List<Item>();
            filteredItems = allItems;

            foreach (var item in allItems)
            {
                if (!categorySeed.Any(s => s.Level1 == item.Cat1 && s.Level2 == item.Cat2 && s.Level3 == item.Cat3 && s.Level4 == item.Cat4))
                {
                    categorySeed.Add(new ETAG_ERP.Helpers.CategorySeedData(item.Cat1, item.Cat2, item.Cat3, item.Cat4, null, null));
                }
            }

            ResetSelection();
            ShowCurrentLevel();
        }

        private void ResetSelection()
        {
            _currentLevel = 1;
            _selectedLevel1 = null;
            _selectedLevel2 = null;
            _selectedLevel3 = null;
            _selectedLevel4 = null;
            _currentFamilyIndex = 0;

            _currentFamilies = categorySeed
                .Where(s => !string.IsNullOrWhiteSpace(s.Level1))
                .Select(s => s.Level1)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        private void BackCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_currentLevel > 1)
            {
                _currentLevel--;
                ShowCurrentLevel();
            }
        }

        private void NextCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_currentLevel == 1 && _currentFamilies.Count > 0)
            {
                _currentFamilyIndex++;
                if (_currentFamilyIndex >= _currentFamilies.Count) _currentFamilyIndex = 0;

                _selectedLevel1 = _currentFamilies[_currentFamilyIndex];
                _currentLevel = 2;
                ShowCurrentLevel();
            }
        }

        private void ProductSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ProductSearchTextBox.Text.ToLower();
            filteredItems = string.IsNullOrWhiteSpace(searchText)
                ? allItems
                : allItems.Where(i => i.ItemName.ToLower().Contains(searchText)).ToList();

            ShowCurrentLevel();
        }

        #endregion

        private void Show_Edit_Item_Click(object sender, RoutedEventArgs e)
        {
            // فتح شاشة إدارة الأصناف كنافذة مستقلة
            var window = new ItemsManagementWindow();
            window.ShowDialog();  // ShowDialog عشان تبقى modal وتوقف التفاعل مع الكاتيجوري لحد ما تقفلها
        }
    }

}
