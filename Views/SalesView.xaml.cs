using ETAG_ERP.Commands;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ETAG_ERP.Views
{
    public partial class SalesView : UserControl
    {
        public SalesView()
        {
            InitializeComponent();
            DataContext = new SalesViewModel();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // يمكن إضافة أي تعامل إضافي عند تغيير الاختيار إذا احتجنا
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class SalesViewModel : INotifyPropertyChanged
    {
        private string dbPath = "Database.db";

        public ObservableCollection<ItemModel> Items { get; set; } = new ObservableCollection<ItemModel>();
        public ObservableCollection<InvoiceLineModel> SelectedItems { get; set; } = new ObservableCollection<InvoiceLineModel>();

        private string _selectedDocumentType = "فاتورة";
        public string SelectedDocumentType
        {
            get => _selectedDocumentType;
            set
            {
                _selectedDocumentType = value;
                OnPropertyChanged(nameof(SelectedDocumentType));
            }
        }

        public decimal TotalAmount => SelectedItems.Sum(i => i.Total);

        public ICommand SaveCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand OpenAddNewItemDialogCommand { get; }

        public SalesViewModel()
        {
            EnsureDatabaseExists();
            LoadItems();


            AddItemCommand = new RelayCommand<ItemModel>(AddItemToInvoice);
            RemoveItemCommand = new RelayCommand<InvoiceLineModel>(RemoveItemFromInvoice);
            SelectedItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalAmount));
        }


        private void EnsureDatabaseExists()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                using var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                conn.Open();

                string createItems = @"CREATE TABLE IF NOT EXISTS Items(
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Name TEXT,
                                            Code TEXT,
                                            Category TEXT,
                                            ImagePath TEXT,
                                            PurchasePrice REAL,
                                            Price1 REAL,
                                            Price2 REAL,
                                            Price3 REAL
                                        );";

                string createInvoices = @"CREATE TABLE IF NOT EXISTS Invoices(
                                            InvoiceId INTEGER PRIMARY KEY AUTOINCREMENT,
                                            InvoiceDate TEXT,
                                            DocumentType TEXT
                                         );";

                string createInvoiceLines = @"CREATE TABLE IF NOT EXISTS InvoiceLines(
                                                LineId INTEGER PRIMARY KEY AUTOINCREMENT,
                                                InvoiceId INTEGER,
                                                ItemId INTEGER,
                                                Name TEXT,
                                                Quantity INTEGER,
                                                Price REAL,
                                                FOREIGN KEY(InvoiceId) REFERENCES Invoices(InvoiceId)
                                            );";

                using var cmd = new SQLiteCommand(createItems, conn);
                cmd.ExecuteNonQuery();

                cmd.CommandText = createInvoices;
                cmd.ExecuteNonQuery();

                cmd.CommandText = createInvoiceLines;
                cmd.ExecuteNonQuery();
            }
        }

        public void LoadItems()
        {
            Items.Clear();
            try
            {
                using var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                conn.Open();

                string query = "SELECT Id, Name, Code, Category, ImagePath, PurchasePrice, Price1, Price2, Price3 FROM Items";

                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Items.Add(new ItemModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Code = reader["Code"].ToString(),
                        Category = reader["Category"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                        PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                        Price1 = Convert.ToDecimal(reader["Price1"]),
                        Price2 = Convert.ToDecimal(reader["Price2"]),
                        Price3 = Convert.ToDecimal(reader["Price3"]),
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تحميل الأصناف: " + ex.Message);
            }
        }

        public void AddNewItem(ItemModel newItem)
        {
            if (newItem == null) return;

            try
            {
                using var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                conn.Open();

                string insert = @"INSERT INTO Items (Name, Code, Category, ImagePath, PurchasePrice, Price1, Price2, Price3)
                                  VALUES (@name, @code, @cat, @img, @pp, @p1, @p2, @p3);
                                  SELECT last_insert_rowid();";

                using var cmd = new SQLiteCommand(insert, conn);
                cmd.Parameters.AddWithValue("@name", newItem.Name);
                cmd.Parameters.AddWithValue("@code", newItem.Code);
                cmd.Parameters.AddWithValue("@cat", newItem.Category);
                cmd.Parameters.AddWithValue("@img", newItem.ImagePath ?? "");
                cmd.Parameters.AddWithValue("@pp", newItem.PurchasePrice);
                cmd.Parameters.AddWithValue("@p1", newItem.Price1);
                cmd.Parameters.AddWithValue("@p2", newItem.Price2);
                cmd.Parameters.AddWithValue("@p3", newItem.Price3);

                newItem.Id = Convert.ToInt32(cmd.ExecuteScalar());
                Items.Add(newItem); // يظهر مباشرة في UI
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء إضافة الصنف: " + ex.Message);
            }
        }

        private void AddItemToInvoice(ItemModel item)
        {
            if (item == null) return;

            var existing = SelectedItems.FirstOrDefault(i => i.ItemId == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                SelectedItems.Add(new InvoiceLineModel
                {
                    ItemId = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    Quantity = 1,
                    Price = item.Price1
                });
            }
            OnPropertyChanged(nameof(TotalAmount));
        }

        private void RemoveItemFromInvoice(InvoiceLineModel line)
        {
            if (line == null) return;
            SelectedItems.Remove(line);
            OnPropertyChanged(nameof(TotalAmount));
        }

        private void SaveInvoice()
        {
            try
            {
                using var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                conn.Open();

                using var trans = conn.BeginTransaction();

                string insertInvoice = "INSERT INTO Invoices (InvoiceDate, DocumentType) VALUES (@date, @doctype); SELECT last_insert_rowid();";
                long invoiceId;
                using (var cmd = new SQLiteCommand(insertInvoice, conn))
                {
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@doctype", SelectedDocumentType);
                    invoiceId = (long)cmd.ExecuteScalar();
                }

                foreach (var line in SelectedItems)
                {
                    string insertLine = @"INSERT INTO InvoiceLines (InvoiceId, ItemId, Name, Quantity, Price) 
                                          VALUES (@invoiceId, @itemId, @name, @qty, @price)";
                    using var cmd = new SQLiteCommand(insertLine, conn);
                    cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
                    cmd.Parameters.AddWithValue("@itemId", line.ItemId);
                    cmd.Parameters.AddWithValue("@name", line.Name);
                    cmd.Parameters.AddWithValue("@qty", line.Quantity);
                    cmd.Parameters.AddWithValue("@price", line.Price);
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();

                SelectedItems.Clear();
                OnPropertyChanged(nameof(TotalAmount));

                MessageBox.Show("تم حفظ الفاتورة بنجاح.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء حفظ الفاتورة: " + ex.Message);
            }
        }

        private void PrintInvoice()
        {
            try
            {
                PrintDialog printDlg = new PrintDialog();
                if (printDlg.ShowDialog() == true)
                {
                    FlowDocument doc = GeneratePrintDocument();
                    IDocumentPaginatorSource idpSource = doc;
                    printDlg.PrintDocument(idpSource.DocumentPaginator, "طباعة الفاتورة");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء الطباعة: " + ex.Message);
            }
        }

        private FlowDocument GeneratePrintDocument()
        {
            var doc = new FlowDocument { PagePadding = new Thickness(50) };

            var header = new Paragraph(new Run("فاتورة المبيعات"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(header);

            var table = new Table();
            doc.Blocks.Add(table);

            for (int i = 0; i < 5; i++) table.Columns.Add(new TableColumn());

            var headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("الكود"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("الاسم"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("الكمية"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("السعر"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("الإجمالي"))));

            var headerRowGroup = new TableRowGroup();
            headerRowGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerRowGroup);

            var bodyRows = new TableRowGroup();
            foreach (var item in SelectedItems)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Code ?? ""))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Name ?? ""))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Quantity.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Price.ToString("C")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Total.ToString("C")))));
                bodyRows.Rows.Add(row);
            }
            table.RowGroups.Add(bodyRows);

            var totalPara = new Paragraph(new Run($"الإجمالي النهائي: {TotalAmount:C}"))
            {
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };
            doc.Blocks.Add(totalPara);

            return doc;
        }

        private void ExportInvoice()
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    FileName = "InvoiceExport.csv"
                };

                if (dlg.ShowDialog() == true)
                {
                    using var writer = new StreamWriter(dlg.FileName);

                    writer.WriteLine("الكود,الاسم,الكمية,السعر,الإجمالي");

                    foreach (var line in SelectedItems)
                    {
                        writer.WriteLine($"{line.Code},{line.Name},{line.Quantity},{line.Price},{line.Total}");
                    }

                    writer.WriteLine($",,,الإجمالي النهائي,{TotalAmount}");

                    MessageBox.Show("تم التصدير بنجاح.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء التصدير: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public class ItemModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 { get; set; }
    }

    public class InvoiceLineModel : INotifyPropertyChanged
    {
        public int ItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 1) value = 1;
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
