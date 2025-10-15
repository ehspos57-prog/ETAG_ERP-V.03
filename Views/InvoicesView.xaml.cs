using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents; // للطباعة

namespace ETAG_ERP.Views
{
    public partial class InvoicesView : UserControl
    {
        private List<Invoice> invoices;

        public InvoicesView()
        {
            InitializeComponent();
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            try
            {
                invoices = DatabaseHelper.GetAllInvoices(); // تأكد أن الدالة موجودة في DatabaseHelper
                InvoicesGrid.ItemsSource = invoices;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل الفواتير:\n" + ex.Message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            // TODO: تنفيذ حذف الفاتورة
        }

        private void ExportInvoices_Click(object sender, RoutedEventArgs e)
        {
            // TODO: تنفيذ تصدير الفواتير
        }

        private void RefreshInvoices_Click(object sender, RoutedEventArgs e)
        {
            LoadInvoices(); // إعادة تحميل البيانات
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: تنفيذ البحث حسب النص المدخل
        }

        private void InvoicesGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // TODO: التعامل مع تعديل الخلايا
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesGrid.SelectedItem is Invoice selectedInvoice)
            {
                var detailsWindow = new InvoiceDetailsView(selectedInvoice);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("من فضلك اختر فاتورة لعرض التفاصيل.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PrintInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesGrid.SelectedItem is Invoice selectedInvoice)
            {
                PrintInvoice(selectedInvoice);
            }
            else
            {
                MessageBox.Show("من فضلك اختر فاتورة للطباعة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PrintInvoice(Invoice invoice)
        {
            try
            {
                PrintDialog printDlg = new PrintDialog();
                if (printDlg.ShowDialog() == true)
                {
                    FlowDocument doc = new FlowDocument();
                    Paragraph p = new Paragraph();
                    p.Inlines.Add(new Bold(new Run("فاتورة رقم: " + invoice.InvoiceNumber + "\n")));
                    p.Inlines.Add(new Run("التاريخ: " + invoice.Date.ToShortDateString() + "\n"));
                    p.Inlines.Add(new Run("العميل: " + invoice.ClientName + "\n"));
                    p.Inlines.Add(new Run("الإجمالي: " + invoice.TotalAmount.ToString("C") + "\n"));
                    p.Inlines.Add(new Run("الحالة: " + invoice.Status + "\n"));
                    doc.Blocks.Add(p);

                    IDocumentPaginatorSource idpSource = doc;
                    printDlg.PrintDocument(idpSource.DocumentPaginator, "طباعة الفاتورة");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء الطباعة: " + ex.Message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddInvoice_Click(object sender, RoutedEventArgs e)
        {
            var salesWindow = new Window()
            {
                Title = "بيع - فاتورة جديدة",
                Content = new SalesView(), // تأكد أن SalesView موجودة كـ UserControl
                Width = 900,
                Height = 700
            };
            salesWindow.Show();
        }
    }
}
