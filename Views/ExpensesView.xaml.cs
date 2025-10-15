using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class ExpensesView : UserControl
    {
        public ObservableCollection<Expense> ExpensesList { get; set; } = new ObservableCollection<Expense>();

        public ExpensesView()
        {
            InitializeComponent();
            LoadSampleData();
            ExpensesGrid.ItemsSource = ExpensesList;
        }

        private void LoadSampleData()
        {
            ExpensesList.Clear();
            ExpensesList.Add(new Expense { Date = DateTime.Today, Category = "إيجار", Description = "إيجار المكتب", Amount = 1500, Username = "admin" });
            ExpensesList.Add(new Expense { Date = DateTime.Today.AddDays(-2), Category = "كهرباء", Description = "فاتورة كهرباء", Amount = 600, Username = "mohamed" });
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم تطبيق الفلاتر.");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("إضافة مصروف.");
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ExpensesGrid.SelectedItem is Expense expense)
                MessageBox.Show($"تعديل المصروف: {expense.Description}");
            else
                MessageBox.Show("اختر مصروفًا للتعديل.");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ExpensesGrid.SelectedItem is Expense expense)
            {
                if (MessageBox.Show($"هل تريد حذف: {expense.Description}؟", "تأكيد", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    ExpensesList.Remove(expense);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSampleData();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تصدير إلى Excel أو PDF.");
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("طباعة المصروفات.");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (parentWindow is MainWindow mainWindow)
            {
                mainWindow.ContentArea.Content = null;
            }
        }
    }
}
