using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ETAG_ERP.Views
{
    public partial class ReturnsView : System.Windows.Controls.UserControl
    {
        public ReturnsView()
        {
            InitializeComponent();
            this.DataContext = new ReturnsViewModel();
        }
    }

    public class ReturnsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // ======================
        // خصائص الواجهة
        // ======================
        private ObservableCollection<Invoice> _returnList;
        public ObservableCollection<Invoice> ReturnList
        {
            get => _returnList;
            set { _returnList = value; OnPropertyChanged(nameof(ReturnList)); }
        }

        private ObservableCollection<Invoice> AllReturns { get; set; } = new();

        private ObservableCollection<string> _clients;
        public ObservableCollection<string> Clients
        {
            get => _clients;
            set { _clients = value; OnPropertyChanged(nameof(Clients)); }
        }

        private Invoice _selectedReturn;
        public Invoice SelectedReturn
        {
            get => _selectedReturn;
            set
            {
                _selectedReturn = value;
                OnPropertyChanged(nameof(SelectedReturn));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set { _fromDate = value; OnPropertyChanged(nameof(FromDate)); }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set { _toDate = value; OnPropertyChanged(nameof(ToDate)); }
        }

        private string _selectedClient;
        public string SelectedClient
        {
            get => _selectedClient;
            set { _selectedClient = value; OnPropertyChanged(nameof(SelectedClient)); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
        }

        // ======================
        // constructor
        // ======================
        public ReturnsViewModel()
        {
            LoadReturnsData();
        }

        // ======================
        // تحميل البيانات التجريبية
        // ======================
        private void LoadReturnsData()
        {
            AllReturns.Clear();

            // مثال بيانات مبدئية
            AllReturns.Add(new Invoice { InvoiceNumber = "R001", ClientName = "عميل 1", Date = DateTime.Today, TotalAmount = 100, Description = "مرتجع بسيط", Username = "user1" });
            AllReturns.Add(new Invoice { InvoiceNumber = "R002", ClientName = "عميل 2", Date = DateTime.Today.AddDays(-1), TotalAmount = 250, Description = "مرتجع منتجات تالفة", Username = "user2" });

            ReturnList = new ObservableCollection<Invoice>(AllReturns);

            Clients = new ObservableCollection<string> { "عميل 1", "عميل 2", "عميل 3" };
        }

        // ======================
        // الأوامر
        // ======================


        // ======================
        // تنفيذ الأوامر
        // ======================
        private void ExecuteFilter()
        {
            var filtered = AllReturns.AsEnumerable();

            if (FromDate.HasValue)
                filtered = filtered.Where(r => r.Date >= FromDate.Value);

            if (ToDate.HasValue)
                filtered = filtered.Where(r => r.Date <= ToDate.Value);

            if (!string.IsNullOrWhiteSpace(SelectedClient))
                filtered = filtered.Where(r => r.ClientName == SelectedClient);

            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(r =>
                    (r.InvoiceNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.ClientName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

            ReturnList = new ObservableCollection<Invoice>(filtered);

            ShowMessage($"تمت التصفية: {ReturnList.Count} نتيجة");
        }

        private void ExecuteAdd()
        {
            ShowMessage("فتح نافذة إضافة مرتجع (لم يتم تنفيذها بعد)");
        }

        private bool CanEditOrDelete() => SelectedReturn != null;

        private void ExecuteEdit()
        {
            if (SelectedReturn == null)
            {
                ShowMessage("يرجى اختيار المرتجع أولاً");
                return;
            }
            ShowMessage($"فتح نافذة تعديل المرتجع رقم: {SelectedReturn.InvoiceNumber} (لم يتم تنفيذها بعد)");
        }

        private void ExecuteDelete()
        {
            if (SelectedReturn == null)
            {
                ShowMessage("يرجى اختيار المرتجع أولاً");
                return;
            }

            if (MessageBox.Show($"هل تريد حذف المرتجع رقم {SelectedReturn.InvoiceNumber}؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                AllReturns.Remove(SelectedReturn);
                ReturnList.Remove(SelectedReturn);
                SelectedReturn = null;
                ShowMessage("تم حذف المرتجع.");
            }
        }

        private void ExecuteRefresh()
        {
            LoadReturnsData();
            ShowMessage("تم تحديث البيانات.");
        }

        private void ExecuteExport()
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    FileName = "مرتجعات",
                    DefaultExt = ".csv",
                    Filter = "CSV files (.csv)|*.csv|All files (*.*)|*.*"
                };

                if (dlg.ShowDialog() == true)
                {
                    ExportToCsv(dlg.FileName);
                    ShowMessage($"تم التصدير بنجاح إلى الملف:\n{dlg.FileName}");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"حدث خطأ أثناء التصدير:\n{ex.Message}");
            }
        }

        private void ExportToCsv(string filePath)
        {
            var csvLines = new ObservableCollection<string>();
            csvLines.Add("رقم المرتجع,التاريخ,العميل,الإجمالي,المستخدم,الوصف");

            foreach (var r in ReturnList)
            {
                string line = $"{r.InvoiceNumber},{r.Date:yyyy-MM-dd},{r.ClientName},{r.TotalAmount},{r.Username},{r.Description}";
                csvLines.Add(line);
            }

            File.WriteAllLines(filePath, csvLines, Encoding.UTF8);
        }

        private void ExecutePrint()
        {
            ShowMessage("تم إرسال بيانات المرتجعات إلى الطابعة (تنفيذ الطباعة الفعلية غير مفعّل)");
        }

        private bool CanOpenDetails() => SelectedReturn != null;

        private void ExecuteOpenDetails()
        {
            if (SelectedReturn == null)
            {
                ShowMessage("يرجى اختيار المرتجع أولاً");
                return;
            }
            var window = new InvoiceDetailsWindow(SelectedReturn);
            window.Show();
        }

        private void ShowMessage(string msg) => MessageBox.Show(msg);
    }

}
