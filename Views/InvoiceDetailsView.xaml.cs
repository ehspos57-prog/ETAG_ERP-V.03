using ClosedXML.Excel;
using ETAG_ERP.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;

namespace ETAG_ERP.Views
{
    public partial class InvoiceDetailsWindow : Window
    {
        public Invoice Invoice { get; set; }
        public decimal TotalAmount { get; private set; }

        public InvoiceDetailsWindow(int invoiceId)
        {
            InitializeComponent();

            try
            {
                // جلب البيانات من قاعدة البيانات
                Invoice = DatabaseHelper.GetInvoiceById(invoiceId);

                if (Invoice == null) // لو مش لاقي بيانات
                {
                    Invoice = GetDummyInvoice(GetTotalAmount());
                }
            }
            catch
            {
                // لو حصل خطأ في قاعدة البيانات
                Invoice = GetDummyInvoice(GetTotalAmount());
            }

            DataContext = this;
        }

        public InvoiceDetailsWindow(Invoice invoice)
        {
            Invoice = invoice;
        }

        // زر الإغلاق
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // زر الطباعة
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument doc = BuildInvoiceDocument();

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                IDocumentPaginatorSource dps = doc;
                pd.PrintDocument(dps.DocumentPaginator, "طباعة فاتورة");
            }
        }

        // زر التصدير PDF
        private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = $"Invoice_{Invoice.InvoiceNumber}.pdf";
                FileHelper.ExportInvoiceToPdf(Invoice, filePath);
                MessageBox.Show($"تم حفظ الفاتورة في {filePath}", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تصدير PDF: " + ex.Message);
            }
        }

        // زر التصدير Excel
        private void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = $"Invoice_{Invoice.InvoiceNumber}.xlsx";
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.AddWorksheet("Invoice");

                    ws.Cell(1, 1).Value = "رقم الفاتورة";
                    ws.Cell(1, 2).Value = Invoice.InvoiceNumber;
                    ws.Cell(2, 1).Value = "العميل";
                    ws.Cell(2, 2).Value = Invoice.ClientName;
                    ws.Cell(3, 1).Value = "التاريخ";
                    ws.Cell(3, 2).Value = Invoice.Date.ToString("dd/MM/yyyy");
                    ws.Cell(4, 1).Value = "الإجمالي";
                    ws.Cell(4, 2).Value = Invoice.TotalAmount;

                    int row = 6;
                    ws.Cell(row, 1).Value = "الكود";
                    ws.Cell(row, 2).Value = "الصنف";
                    ws.Cell(row, 3).Value = "الكمية";
                    ws.Cell(row, 4).Value = "سعر الوحدة";
                    ws.Cell(row, 5).Value = "الخصم";
                    ws.Cell(row, 6).Value = "الإجمالي";

                    foreach (var item in Invoice.Items)
                    {
                        row++;
                        ws.Cell(row, 1).Value = item.ItemCode;
                        ws.Cell(row, 2).Value = item.ItemName;
                        ws.Cell(row, 3).Value = item.Quantity;
                        ws.Cell(row, 4).Value = item.UnitPrice;
                        ws.Cell(row, 5).Value = item.Discount;
                        ws.Cell(row, 6).Value = item.Total;
                    }

                    workbook.SaveAs(filePath);
                }

                MessageBox.Show($"تم حفظ الفاتورة في {filePath}", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تصدير Excel: " + ex.Message);
            }
        }

        // زر التصدير XML للبوابة
        private void ExportXmlButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = $"Invoice_{Invoice.InvoiceNumber}.xml";

                XElement xml = new XElement("Invoice",
                    new XElement("InvoiceNumber", Invoice.InvoiceNumber),
                    new XElement("Client", Invoice.ClientName),
                    new XElement("Date", Invoice.Date),
                    new XElement("Total", Invoice.TotalAmount),
                    new XElement("Items",
                        new List<XElement>(
                            Invoice.Items.ConvertAll(item =>
                                new XElement("Item",
                                    new XElement("Code", item.ItemCode),
                                    new XElement("Name", item.ItemName),
                                    new XElement("Qty", item.Quantity),
                                    new XElement("UnitPrice", item.UnitPrice),
                                    new XElement("Discount", item.Discount),
                                    new XElement("Total", item.Total)
                                )
                            )
                        )
                    )
                );

                xml.Save(filePath);

                MessageBox.Show($"تم حفظ XML في {filePath}", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تصدير XML: " + ex.Message);
            }
        }

        // بناء مستند الطباعة
        private FlowDocument BuildInvoiceDocument()
        {
            FlowDocument doc = new FlowDocument
            {
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 14,
                PagePadding = new Thickness(50),
                FlowDirection = FlowDirection.RightToLeft
            };

            doc.Blocks.Add(new Paragraph(new Run($"رقم الفاتورة: {Invoice.InvoiceNumber}")));
            doc.Blocks.Add(new Paragraph(new Run($"العميل: {Invoice.ClientName}")));
            doc.Blocks.Add(new Paragraph(new Run($"التاريخ: {Invoice.Date:dd/MM/yyyy}")));
            doc.Blocks.Add(new Paragraph(new Run($"الإجمالي: {Invoice.TotalAmount:F2}")));

            Table table = new Table();
            doc.Blocks.Add(table);

            for (int i = 0; i < 6; i++) table.Columns.Add(new TableColumn());
            TableRowGroup group = new TableRowGroup();
            table.RowGroups.Add(group);

            group.Rows.Add(new TableRow());
            group.Rows[0].Cells.Add(new TableCell(new Paragraph(new Run("الكود"))));
            group.Rows[0].Cells.Add(new TableCell(new Paragraph(new Run("الصنف"))));
            group.Rows[0].Cells.Add(new TableCell(new Paragraph(new Run("الكمية"))));
            group.Rows[0].Cells.Add(new TableCell(new Paragraph(new Run("سعر الوحدة"))));
            group.Rows[0].Cells.Add(new TableCell(new Paragraph(new Run("الخصم"))));
            group.Rows[0].Cells.Add(new TableCell(new Paragraph(new Run("الإجمالي"))));

            foreach (var item in Invoice.Items)
            {
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ItemCode))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ItemName))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Quantity.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.UnitPrice.ToString("F2")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Discount.ToString("F2")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Total.ToString("F2")))));
                group.Rows.Add(row);
            }

            return doc;
        }

        private decimal GetTotalAmount()
        {
            return TotalAmount;
        }

        // بيانات افتراضية لو مفيش داتا
        private Invoice GetDummyInvoice(decimal totalAmount)
        {
            return new Invoice
            {
                InvoiceNumber = "D-0001",
                ClientName = "عميل تجريبي",
                Date = DateTime.Now,
                totalAmount = 1500,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { ItemCode="IT001", ItemName="منتج تجريبي 1", Quantity=2, UnitPrice=200, Discount=0, },
                    new InvoiceItem { ItemCode="IT002", ItemName="منتج تجريبي 2", Quantity=1, UnitPrice=500, Discount=50, },
                    new InvoiceItem { ItemCode="IT003", ItemName="منتج تجريبي 3", Quantity=3, UnitPrice=250, Discount=0, }
                }
            };
        }
    }
}
