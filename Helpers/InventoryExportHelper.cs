using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ETAG_ERP.Helpers
{
    public static class InventoryExportHelper
    {
        // نموذج بيانات المخزون (استبدله ببيانات حقيقية من قاعدة البيانات)
        private static List<InventoryItem> GetInventoryData()
        {
            return new List<InventoryItem>
            {
                new InventoryItem { ItemCode = "A001", ItemName = "صنف 1", Quantity = 10, Price = 100 },
                new InventoryItem { ItemCode = "A002", ItemName = "صنف 2", Quantity = 5, Price = 250 },
                new InventoryItem { ItemCode = "A003", ItemName = "صنف 3", Quantity = 20, Price = 50 },
            };
        }

        // ======================== EXPORT ========================
        public static void ExportInventoryToExcel()
        {
            try
            {
                var items = GetInventoryData();
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Inventory");
                    ws.Cells[1, 1].Value = "كود الصنف";
                    ws.Cells[1, 2].Value = "اسم الصنف";
                    ws.Cells[1, 3].Value = "الكمية";
                    ws.Cells[1, 4].Value = "السعر";

                    int row = 2;
                    foreach (var item in items)
                    {
                        ws.Cells[row, 1].Value = item.ItemCode;
                        ws.Cells[row, 2].Value = item.ItemName;
                        ws.Cells[row, 3].Value = item.Quantity;
                        ws.Cells[row, 4].Value = item.Price;
                        row++;
                    }

                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Inventory.xlsx");
                    File.WriteAllBytes(filePath, package.GetAsByteArray());

                    MessageBox.Show($"تم تصدير جرد المخزون إلى Excel:\n{filePath}", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تصدير Excel:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ExportInventoryToPdf()
        {
            try
            {
                var items = GetInventoryData();
                Document doc = new Document(PageSize.A4);
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Inventory.pdf");
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                var titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD);
                doc.Add(new iTextSharp.text.Paragraph("جرد المخزون", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20f });

                PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 1.5f, 3f, 1.5f, 2f });
                table.AddCell("كود الصنف");
                table.AddCell("اسم الصنف");
                table.AddCell("الكمية");
                table.AddCell("السعر");

                foreach (var item in items)
                {
                    table.AddCell(item.ItemCode);
                    table.AddCell(item.ItemName);
                    table.AddCell(item.Quantity.ToString());
                    table.AddCell(item.Price.ToString("0.00"));
                }

                doc.Add(table);
                doc.Close();

                MessageBox.Show($"تم تصدير جرد المخزون إلى PDF:\n{filePath}", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تصدير PDF:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ======================== PRINT ========================
        public static void PrintFlowDocument(System.Windows.Documents.FlowDocument doc)
        {
            try
            {
                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "طباعة التقرير");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء الطباعة:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ======================== DATA MODEL ========================
        private class InventoryItem
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
        }
    }
}
