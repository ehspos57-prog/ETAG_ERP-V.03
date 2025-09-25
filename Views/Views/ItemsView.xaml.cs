using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class ItemsView : UserControl
    {
        public ObservableCollection<Item> Items { get; set; }

        public ItemsView()
        {
            InitializeComponent();

            Items = new ObservableCollection<Item>
            {
                new Item {
                    Code = "A1001",
                    Name = "بلسم ناعم",
                    Quantity = 50,
                    PurchasePrice = 20,
                    SellingPrice = 35,
                    Price1 = 30,
                    Price2 = 32,
                    Price3 = 33,
                    CategoryLevel1 = "منتجات العناية",
                    CategoryLevel2 = "شعر",
                    CategoryLevel3 = "بلسم",
                    CategoryLevel4 = "",
                    CategoryLevel5 = ""
                }
            };

            ItemsGrid.ItemsSource = Items;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فتح نافذة إضافة صنف جديد (لاحقاً).");
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is Item selectedItem)
            {
                MessageBox.Show($"تعديل الصنف: {selectedItem.Name}");
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is Item selectedItem)
            {
                if (MessageBox.Show($"هل أنت متأكد من حذف {selectedItem.Name}؟", "تأكيد", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Items.Remove(selectedItem);
                }
            }
        }
    }

    public class Item
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double PurchasePrice { get; set; }
        public double SellingPrice { get; set; }
        public double Price1 { get; set; }
        public double Price2 { get; set; }
        public double Price3 { get; set; }
        public string CategoryLevel1 { get; set; }
        public string CategoryLevel2 { get; set; }
        public string CategoryLevel3 { get; set; }
        public string CategoryLevel4 { get; set; }
        public string CategoryLevel5 { get; set; }
        public string ItemName { get; internal set; }
        public string Cat1 { get; internal set; }
        public string Cat2 { get; internal set; }
        public string Cat3 { get; internal set; }
        public string Cat4 { get; internal set; }
        public string Cat5 { get; internal set; }
        public string ImagePath { get; internal set; }
    }
}
