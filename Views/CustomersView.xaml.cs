using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ETAG_ERP.Views
{
    public partial class CustomersView : UserControl, INotifyPropertyChanged
    {
        // المصدر الرئيسي المعروض على الجريد
        public ObservableCollection<Client> ClientsList { get; set; } = new ObservableCollection<Client>();

        // نسخة أصلية لاسترجاع التعديلات
        private ObservableCollection<Client> OriginalClientsList = new ObservableCollection<Client>();

        // View للتصفية/الفرز بدون تعديل المصدر
        private ICollectionView _clientsView;

        // خصائص الملخص
        private int _clientsCount;
        public int ClientsCount
        {
            get => _clientsCount;
            set { _clientsCount = value; OnPropertyChanged(nameof(ClientsCount)); }
        }

        private decimal _totalBalance;
        public decimal TotalBalance
        {
            get => _totalBalance;
            set { _totalBalance = value; OnPropertyChanged(nameof(TotalBalance)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CustomersView()
        {
            InitializeComponent();
            DataContext = this;

            // ربط الـ view
            _clientsView = CollectionViewSource.GetDefaultView(ClientsList);
            _clientsView.Filter = FilterPredicate;

            // مراقبة تغيّر مجموعة العملاء لتحديث الملخص
            ClientsList.CollectionChanged += (_, __) => RefreshSummary();
            LoadClientsFromDatabase();
        }

        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #region Loading / Summary

        private void LoadClientsFromDatabase()
        {
            try
            {
                ClientsList.Clear();
                var clients = DatabaseHelper.GetAllClients() ?? new List<Client>();

                foreach (var c in clients)
                    ClientsList.Add(c);

                CloneToOriginalList();
                RefreshSummary();
                _clientsView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل العملاء:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloneToOriginalList()
        {
            OriginalClientsList.Clear();
            foreach (var c in ClientsList)
            {
                OriginalClientsList.Add(new Client
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    Address = c.Address,
                    Email = c.Email,
                    Notes = c.Notes,
                    Balance = c.Balance
                });
            }
        }

        private void RefreshSummary()
        {
            ClientsCount = ClientsList.Count;
            TotalBalance = ClientsList.Sum(x => x?.Balance ?? 0m);
        }

        #endregion

        #region Filtering

        private bool FilterPredicate(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchBox?.Text))
                return true;

            var txt = SearchBox.Text.Trim();
            if (obj is not Client c) return false;

            return (c.Name ?? "").IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0
                || (c.Phone ?? "").IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0
                || (c.Address ?? "").IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _clientsView.Refresh();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _clientsView.Refresh();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            _clientsView.Refresh();
        }

        #endregion

        #region CRUD

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            // نافذة إضافة (يفضل تكون عندك) — أو أضف عنصر فارغ للتحرير المباشر
            var win = new AddClientWindow();
            if (win.ShowDialog() == true)
            {
                LoadClientsFromDatabase();
            }
        }

        private Client GetSelectedClient()
        {
            return ClientsGrid?.SelectedItem as Client;
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedClient();
            if (selected == null)
            {
                MessageBox.Show("اختر عميلًا للتعديل.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // فتح شاشة التعديل بنفس نافذة الإضافة (تمرير نسخة لتجنب تعديل المرجع مباشرة)
            var clone = new Client
            {
                Id = selected.Id,
                Name = selected.Name,
                Phone = selected.Phone,
                Address = selected.Address,
                Email = selected.Email,
                Notes = selected.Notes,
                Balance = selected.Balance
            };

            var win = new AddClientWindow(clone);
            if (win.ShowDialog() == true)
            {
                // حفظ التعديلات في DB ثم إعادة التحميل
                try
                {
                    DatabaseHelper.UpdateClient(clone);
                    LoadClientsFromDatabase();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"تعذر حفظ التعديل:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedClient();
            if (selected == null)
            {
                MessageBox.Show("اختر عميلًا للحذف.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show($"هل أنت متأكد من حذف العميل:\n{selected.Name} ؟",
                                          "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                // نحاول حسب توقيع الدالة المتاح لديك
                try { DatabaseHelper.DeleteClient(selected.Id); }
                catch { DatabaseHelper.DeleteClient(selected); }

                ClientsList.Remove(selected);
                RefreshSummary();
                CloneToOriginalList();
                MessageBox.Show("تم حذف العميل.", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"تعذر حذف العميل:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveClients_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تحقق بسيط قبل الحفظ
                foreach (var c in ClientsList)
                {
                    if (string.IsNullOrWhiteSpace(c.Name))
                        throw new Exception("لا يمكن حفظ سجل بدون اسم.");
                    if (!string.IsNullOrWhiteSpace(c.Phone) && c.Phone.Length < 5)
                        throw new Exception($"رقم هاتف غير صالح للعميل: {c.Name}");
                }

                foreach (var c in ClientsList)
                {
                    if (c.Id > 0)
                        DatabaseHelper.UpdateClient(c);
                    else
                        DatabaseHelper.InsertClient(c);
                }

                CloneToOriginalList();
                RefreshSummary();
                MessageBox.Show("تم حفظ التغييرات بنجاح.", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الحفظ:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelChanges_Click(object sender, RoutedEventArgs e)
        {
            // رجّع النسخة الأصلية
            ClientsList.Clear();
            foreach (var c in OriginalClientsList)
                ClientsList.Add(new Client
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    Address = c.Address,
                    Email = c.Email,
                    Notes = c.Notes,
                    Balance = c.Balance
                });

            _clientsView.Refresh();
            RefreshSummary();
            MessageBox.Show("تم التراجع عن التعديلات.", "تراجع", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Grid UX & Validation

        private void ClientsGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // منع القائمة إن مفيش تحديد
            if (GetSelectedClient() == null) e.Handled = true;
        }

        private void ClientsGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // ممكن تحجز أعمدة معينة من التعديل إن حبيت
            // مثال: منع تعديل الـ Id لو معروض
            // if ((e.Column as DataGridTextColumn)?.Binding is Binding b && (b.Path.Path == "Id")) e.Cancel = true;
        }

        private void ClientsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // فور انتهاء التحرير، نظّف القيم وعدّل الملخص
            if (e.Column.Header?.ToString() == "الرصيد")
            {
                if (e.EditingElement is TextBox tb)
                {
                    if (!decimal.TryParse(tb.Text, out var val))
                    {
                        MessageBox.Show("قيمة الرصيد غير صالحة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                        tb.Text = "0";
                    }
                }
            }

            // تحديث الملخص بعد أي تغيير
            // (نعطي Dispatcher عشان يتم حفظ القيمة في الكائن أولاً)
            Dispatcher.BeginInvoke(new Action(RefreshSummary));
        }

        #endregion
    }
}
