using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using ETAG_ERP.Views;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ETAG_ERP.ViewModels
{
    public class LedgerViewModel : BaseViewModel
    {
        public ObservableCollection<LedgerEntry> LedgerList { get; set; } = new();
        public LedgerEntry? SelectedEntry { get; set; }

        public ObservableCollection<string> LedgerTypes { get; set; } =
            new(new[] { "الكل", "عميل", "مورد", "مشروع", "مصروفات" });

        public string SelectedLedgerType { get; set; } = "الكل";

        public ObservableCollection<Account> AccountsList { get; set; } =
            new(new[]
            {
                new Account { Id=1, Name="عميل أحمد" },
                new Account { Id=2, Name="مشروع مول القاهرة" },
                new Account { Id=3, Name="مصروفات انتقال" },
            });

        public Account? SelectedAccount { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SearchText { get; set; } = string.Empty;

        private string _totalSummary = "";
        public string TotalSummary
        {
            get => _totalSummary;
            set { _totalSummary = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand FilterCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand PrintCommand { get; }

        public LedgerViewModel()
        {
            FilterCommand = new RelayCommand(LoadData);                // بدون (_) لأن LoadData() ما بتاخدش باراميتر
            ClearFilterCommand = new RelayCommand(ClearFilters);
            AddCommand = new RelayCommand(AddEntry);
            EditCommand = new RelayCommand(EditEntry, () => SelectedEntry != null);
            DeleteCommand = new RelayCommand(DeleteEntry, () => SelectedEntry != null);
            RefreshCommand = new RelayCommand(LoadData);
            ExportCommand = new RelayCommand(ExportToExcel);
            PrintCommand = new RelayCommand(PrintLedger);

            LoadData();
        }


        private void LoadData()
        {
            LedgerList.Clear();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Ledger WHERE 1=1";
            if (FromDate.HasValue)
                sql += $" AND Date >= '{FromDate:yyyy-MM-dd}'";
            if (ToDate.HasValue)
                sql += $" AND Date <= '{ToDate:yyyy-MM-dd}'";
            if (!string.IsNullOrWhiteSpace(SearchText))
                sql += $" AND (Description LIKE '%{SearchText}%' OR Reference LIKE '%{SearchText}%')";
            if (SelectedAccount != null)
                sql += $" AND Description LIKE '%{SelectedAccount.Name}%'";

            sql += " ORDER BY Date ASC";

            using var cmd = new SQLiteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            decimal runningBalance = 0, totalDebit = 0, totalCredit = 0;
            while (reader.Read())
            {
                var entry = new LedgerEntry
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    Reference = reader["Reference"].ToString(),
                    Description = reader["Description"].ToString(),
                    Debit = Convert.ToDecimal(reader["Debit"]),
                    Credit = Convert.ToDecimal(reader["Credit"]),
                    Category = reader["Category"].ToString()
                };

                runningBalance += entry.Credit - entry.Debit;
                entry.Balance = runningBalance;

                totalDebit += entry.Debit;
                totalCredit += entry.Credit;

                LedgerList.Add(entry);
            }

            TotalSummary = $"إجمالي مدين: {totalDebit} | إجمالي دائن: {totalCredit} | الصافي: {totalCredit - totalDebit}";
        }

        private void ClearFilters()
        {
            FromDate = null;
            ToDate = null;
            SearchText = "";
            SelectedAccount = null;
            SelectedLedgerType = "الكل";
            LoadData();
        }

        private void AddEntry()
        {
            MessageBox.Show("إضافة عملية جديدة (يتم عمل نافذة إدخال لاحقاً).");
        }

        private void EditEntry()
        {
            if (SelectedEntry == null) return;
            MessageBox.Show($"تعديل العملية رقم {SelectedEntry.Id}.");
        }

        private void DeleteEntry()
        {
            if (SelectedEntry == null) return;

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "DELETE FROM Ledger WHERE Id=@id";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", SelectedEntry.Id);
            cmd.ExecuteNonQuery();

            LoadData();
        }

        private void ExportToExcel()
        {
            SaveFileDialog saveFile = new() { Filter = "Excel Files|*.csv", FileName = "LedgerExport.csv" };
            if (saveFile.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Date,Reference,Description,Debit,Credit,Balance,Category");
                foreach (var e in LedgerList)
                {
                    sb.AppendLine($"{e.Date:yyyy-MM-dd},{e.Reference},{e.Description},{e.Debit},{e.Credit},{e.Balance},{e.Category}");
                }
                File.WriteAllText(saveFile.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("تم التصدير إلى Excel (CSV).");
            }
        }

        private void PrintLedger()
        {
            PrintDialog pd = new();
            if (pd.ShowDialog() == true)
            {
                FlowDocument doc = new();
                doc.Blocks.Add(new Paragraph(new Run("كشف حساب")));
                Table table = new();
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.RowGroups.Add(new TableRowGroup());
                var header = new TableRow();
                header.Cells.Add(new TableCell(new Paragraph(new Run("التاريخ"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("الوصف"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("مدين/دائن"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("الرصيد"))));
                table.RowGroups[0].Rows.Add(header);

                foreach (var e in LedgerList)
                {
                    var row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(e.Date.ToShortDateString()))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(e.Description))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run($"{e.Debit}/{e.Credit}"))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(e.Balance.ToString()))));
                    table.RowGroups[0].Rows.Add(row);
                }

                doc.Blocks.Add(table);
                pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "طباعة كشف الحساب");
            }
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
