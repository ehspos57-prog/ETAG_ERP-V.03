using ETAG_ERP.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ETAG_ERP.Views
{
    public partial class SettingsView : Window
    {
        private string _dbPath = "ETAG_ERP.db";
        private SQLiteConnection _conn;
        private string _backgroundImagePath = "";
        private User CurrentUser = new User { Id = 1, Username = "admin", Role = "Admin" };

        public SettingsView()
        {
            InitializeComponent();
            _conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
            LoadAllData();
            LoadCategoriesTree();
            ApplyUserPermissions();
        }

        #region Load Data
        private void LoadAllData()
        {
            LoadBranches();
            LoadUsers();
            LoadEmployees();
            LoadAccounts();
        }

        private void LoadBranches() => BranchesDataGrid.ItemsSource = ExecuteQuery("SELECT * FROM Branches").DefaultView;
        private void LoadUsers() => UsersDataGrid.ItemsSource = ExecuteQuery("SELECT Id, Username, Role FROM Users").DefaultView;
        private void LoadEmployees() => EmployeesDataGrid.ItemsSource = ExecuteQuery("SELECT * FROM Employees").DefaultView;
        private void LoadAccounts() => AccountsDataGrid.ItemsSource = ExecuteQuery("SELECT * FROM Accounts").DefaultView;

        private TabItem UsersTab => (TabItem)((TabControl)this.Content).Items[1];
        private TabItem BranchesTab => (TabItem)((TabControl)this.Content).Items[0];
        private TreeView CategoriesTreeView => CategoryTreeView;

        private void LoadCategoriesTree()
        {
            CategoriesTreeView.Items.Clear();
            var dt = ExecuteQuery("SELECT * FROM Categories ORDER BY ParentId ASC");
            var lookup = dt.AsEnumerable().ToLookup(r => r.Field<int?>("ParentId"));
            foreach (var root in lookup[null])
                CategoriesTreeView.Items.Add(CreateTreeItem(root, lookup));
        }

        private TreeViewItem CreateTreeItem(DataRow row, ILookup<int?, DataRow> lookup)
        {
            var item = new TreeViewItem
            {
                Header = row["Name"].ToString(),
                Tag = row["Id"]
            };



            // ContextMenu ديناميكي لكل عنصر
            var menu = new ContextMenu();
            menu.Items.Add(new MenuItem { Header = "إضافة فرع فرعي", Command = new RelayCommand(_ => AddCategoryDialog((int?)item.Tag)) });
            menu.Items.Add(new MenuItem { Header = "تعديل", Command = new RelayCommand(_ => EditCategoryDialog(item)) });
            menu.Items.Add(new MenuItem { Header = "حذف", Command = new RelayCommand(_ => DeleteCategory((int)item.Tag)) });
            item.ContextMenu = menu;

            return item;
        }
        #endregion

        #region CRUD Methods
        private DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
        {
            var dt = new DataTable();
            using (var cmd = new SQLiteCommand(query, _conn))
            {
                if (parameters != null)
                    foreach (var p in parameters)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);

                _conn.Open();
                using (var reader = cmd.ExecuteReader()) dt.Load(reader);
                _conn.Close();
            }
            return dt;
        }

        private void ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
        {
            using (var cmd = new SQLiteCommand(query, _conn))
            {
                if (parameters != null)
                    foreach (var p in parameters)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);

                _conn.Open();
                cmd.ExecuteNonQuery();
                _conn.Close();
            }
        }

        #region Branches CRUD
        private void AddBranch(string name) => ExecuteNonQuery("INSERT INTO Branches (Name) VALUES (@Name)", new Dictionary<string, object> { { "@Name", name } });
        private void EditBranch(int id, string name) => ExecuteNonQuery("UPDATE Branches SET Name=@Name WHERE Id=@Id", new Dictionary<string, object> { { "@Name", name }, { "@Id", id } });
        private void DeleteBranch(int id) => ExecuteNonQuery("DELETE FROM Branches WHERE Id=@Id", new Dictionary<string, object> { { "@Id", id } });
        #endregion

        #region Users CRUD
        private void AddUser(string username, string password, string role) => ExecuteNonQuery(
            "INSERT INTO Users (Username,Password,Role) VALUES (@Username,@Password,@Role)",
            new Dictionary<string, object> { { "@Username", username }, { "@Password", password }, { "@Role", role } });

        private void EditUser(int id, string username, string password, string role) => ExecuteNonQuery(
            "UPDATE Users SET Username=@Username,Password=@Password,Role=@Role WHERE Id=@Id",
            new Dictionary<string, object> { { "@Username", username }, { "@Password", password }, { "@Role", role }, { "@Id", id } });

        private void DeleteUser(int id) => ExecuteNonQuery("DELETE FROM Users WHERE Id=@Id", new Dictionary<string, object> { { "@Id", id } });
        #endregion

        #region Employees CRUD
        private void AddEmployee(string name, int branchId) => ExecuteNonQuery(
            "INSERT INTO Employees (Name,BranchId) VALUES (@Name,@BranchId)",
            new Dictionary<string, object> { { "@Name", name }, { "@BranchId", branchId } });

        private void EditEmployee(int id, string name, int branchId) => ExecuteNonQuery(
            "UPDATE Employees SET Name=@Name,BranchId=@BranchId WHERE Id=@Id",
            new Dictionary<string, object> { { "@Name", name }, { "@BranchId", branchId }, { "@Id", id } });

        private void DeleteEmployee(int id) => ExecuteNonQuery("DELETE FROM Employees WHERE Id=@Id", new Dictionary<string, object> { { "@Id", id } });
        #endregion

        #region Accounts CRUD
        private void AddAccount(string name) => ExecuteNonQuery("INSERT INTO Accounts (Name) VALUES (@Name)", new Dictionary<string, object> { { "@Name", name } });
        private void EditAccount(int id, string name) => ExecuteNonQuery("UPDATE Accounts SET Name=@Name WHERE Id=@Id", new Dictionary<string, object> { { "@Name", name }, { "@Id", id } });
        private void DeleteAccount(int id) => ExecuteNonQuery("DELETE FROM Accounts WHERE Id=@Id", new Dictionary<string, object> { { "@Id", id } });
        #endregion

        #region Categories CRUD
        private void AddCategoryDialog(int? parentId)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("أدخل اسم التصنيف الجديد:", "إضافة تصنيف", "تصنيف جديد");
            if (!string.IsNullOrWhiteSpace(input)) AddCategory(input, parentId);
        }

        private void EditCategoryDialog(TreeViewItem item)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("عدل اسم التصنيف:", "تعديل التصنيف", item.Header.ToString());
            if (!string.IsNullOrWhiteSpace(input)) EditCategory((int)item.Tag, input);
        }

        private void AddCategory(string name, int? parentId)
        {
            ExecuteNonQuery("INSERT INTO Categories (Name, ParentId) VALUES (@Name,@ParentId)",
                new Dictionary<string, object> { { "@Name", name }, { "@ParentId", (object?)parentId ?? DBNull.Value } });
            LoadCategoriesTree();
        }

        private void EditCategory(int id, string name)
        {
            ExecuteNonQuery("UPDATE Categories SET Name=@Name WHERE Id=@Id",
                new Dictionary<string, object> { { "@Name", name }, { "@Id", id } });
            LoadCategoriesTree();
        }

        private void DeleteCategory(int id)
        {
            if (MessageBox.Show("تأكيد الحذف؟", "حذف", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ExecuteNonQuery("DELETE FROM Categories WHERE Id=@Id", new Dictionary<string, object> { { "@Id", id } });
                LoadCategoriesTree();
            }
        }
        #endregion
        #region Button Event Handlers

        // ---------- General Settings ----------
        private void SaveGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم حفظ إعدادات البرنامج العامة.");
        }

        private void SaveAccountsSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم حفظ إعدادات الحسابات.");
        }

        private void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("هل تريد فعلاً إعادة تعيين قاعدة البيانات؟", "تأكيد", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                MessageBox.Show("تم إعادة تعيين قاعدة البيانات.");
        }

        private void ChangeDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Database files|*.db" };
            if (dlg.ShowDialog() == true)
            {
                _dbPath = dlg.FileName;
                MessageBox.Show($"تم تغيير مسار قاعدة البيانات إلى {_dbPath}");
            }
        }

        // ---------- Branches ----------
        private void BranchesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* تحديث واجهة المستخدم عند اختيار فرع */ }

        private void AddBranch_Click(object sender, RoutedEventArgs e)
        {
            AddBranch("فرع جديد");
            LoadBranches();
        }

        private void EditBranch_Click(object sender, RoutedEventArgs e)
        {
            if (BranchesDataGrid.SelectedItem is DataRowView row)
            {
                EditBranch(Convert.ToInt32(row["Id"]), row["Name"].ToString()!);
                LoadBranches();
            }
        }

        private void DeleteBranch_Click(object sender, RoutedEventArgs e)
        {
            if (BranchesDataGrid.SelectedItem is DataRowView row)
            {
                DeleteBranch(Convert.ToInt32(row["Id"]));
                LoadBranches();
            }
        }

        // ---------- Users ----------
        private void AddUser_Click(object sender, RoutedEventArgs e) { AddUser("newuser", "1234", "User"); LoadUsers(); }
        private void EditUser_Click(object sender, RoutedEventArgs e) { if (UsersDataGrid.SelectedItem is DataRowView row) { EditUser(Convert.ToInt32(row["Id"]), row["Username"].ToString()!, row["Password"].ToString()!, row["Role"].ToString()!); LoadUsers(); } }
        private void DeleteUser_Click(object sender, RoutedEventArgs e) { if (UsersDataGrid.SelectedItem is DataRowView row) { DeleteUser(Convert.ToInt32(row["Id"])); LoadUsers(); } }

        // ---------- Employees ----------
        private void AddEmployee_Click(object sender, RoutedEventArgs e) { AddEmployee("موظف جديد", 1); LoadEmployees(); }
        private void EditEmployee_Click(object sender, RoutedEventArgs e) { if (EmployeesDataGrid.SelectedItem is DataRowView row) { EditEmployee(Convert.ToInt32(row["Id"]), row["Name"].ToString()!, Convert.ToInt32(row["BranchId"])); LoadEmployees(); } }
        private void DeleteEmployee_Click(object sender, RoutedEventArgs e) { if (EmployeesDataGrid.SelectedItem is DataRowView row) { DeleteEmployee(Convert.ToInt32(row["Id"])); LoadEmployees(); } }

        // ---------- Permissions ----------
        private void SavePermissions_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم حفظ الصلاحيات."); }

        // ---------- Background & Images ----------
        private void UploadBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" };
            if (dlg.ShowDialog() == true) _backgroundImagePath = dlg.FileName;
        }

        private void UploadProductImages_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم رفع صور المنتجات."); }

        // ---------- Import / Export ----------
        private void ImportClients_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Excel|*.xlsx" };
            if (dlg.ShowDialog() == true) ImportFromExcel(dlg.FileName, "Clients");
        }

        private void ImportItems_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Excel|*.xlsx" };
            if (dlg.ShowDialog() == true) ImportFromExcel(dlg.FileName, "Items");
        }

        private void ExportAllData_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Excel|*.xlsx" };
            if (dlg.ShowDialog() == true) MessageBox.Show($"تم تصدير جميع البيانات إلى {dlg.FileName}");
        }

        private void UploadPrintTemplate_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم رفع قالب الطباعة."); }
        private void ChooseDefaultPrinter_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم اختيار الطابعة الافتراضية."); }
        private void PreviewPrint_Click(object sender, RoutedEventArgs e) { MessageBox.Show("معاينة الطباعة."); }

        // ---------- Categories ----------
        private void AddSubCategory_Click(object sender, RoutedEventArgs e) { AddCategoryDialog(GetSelectedCategoryId()); }
        private void EditCategory_Click(object sender, RoutedEventArgs e) { if (CategoriesTreeView.SelectedItem is TreeViewItem item) EditCategoryDialog(item); }
        private void DeleteCategory_Click(object sender, RoutedEventArgs e) { if (CategoriesTreeView.SelectedItem is TreeViewItem item) DeleteCategory((int)item.Tag); }
        private void SaveCategories_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم حفظ التصنيفات."); }

        // ---------- Accounts ----------
        private void AddAccount_Click(object sender, RoutedEventArgs e) { AddAccount("حساب جديد"); LoadAccounts(); }
        private void EditAccount_Click(object sender, RoutedEventArgs e) { if (AccountsDataGrid.SelectedItem is DataRowView row) { EditAccount(Convert.ToInt32(row["Id"]), row["Name"].ToString()!); LoadAccounts(); } }
        private void DeleteAccount_Click(object sender, RoutedEventArgs e) { if (AccountsDataGrid.SelectedItem is DataRowView row) { DeleteAccount(Convert.ToInt32(row["Id"])); LoadAccounts(); } }

        #endregion

        // Families & SubFamilies Event Handlers
        private void AddFamily_Click(object sender, RoutedEventArgs e) { }
        private void EditFamily_Click(object sender, RoutedEventArgs e) { }
        private void DeleteFamily_Click(object sender, RoutedEventArgs e) { }

        private void AddSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void EditSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void DeleteSubFamily_Click(object sender, RoutedEventArgs e) { }

        private void AddSubSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void EditSubSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void DeleteSubSubFamily_Click(object sender, RoutedEventArgs e) { }

        private void AddSubSubSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void EditSubSubSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void DeleteSubSubSubFamily_Click(object sender, RoutedEventArgs e) { }

        private void AddSubSubSubSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void EditSubSubSubSubFamily_Click(object sender, RoutedEventArgs e) { }
        private void DeleteSubSubSubSubFamily_Click(object sender, RoutedEventArgs e) { }

        #endregion

        #region Permissions
        private void ApplyUserPermissions()
        {
            if (CurrentUser.Role != "Admin")
            {
                UsersTab.IsEnabled = false;
                BranchesTab.IsEnabled = false;
            }
        }
        #endregion

        #region Export / Import

        private void ExportToExcel(DataGrid grid, string fileName)
        {
            using var package = new ExcelPackage(new FileInfo(fileName));
            var ws = package.Workbook.Worksheets.Add("Export");
            for (int i = 0; i < grid.Columns.Count; i++) ws.Cells[1, i + 1].Value = grid.Columns[i].Header;
            for (int i = 0; i < grid.Items.Count; i++)
            {
                var row = grid.Items[i];
                for (int j = 0; j < grid.Columns.Count; j++)
                {
                    if (grid.Columns[j].GetCellContent(row) is TextBlock tb) ws.Cells[i + 2, j + 1].Value = tb.Text;
                }
            }
            package.Save();
        }

        private void ImportFromExcel(string filePath, string type)
        {
            if (!File.Exists(filePath)) { MessageBox.Show("الملف غير موجود"); return; }
            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets[0];
            for (int row = 2; row <= ws.Dimension.End.Row; row++)
            {
                if (type == "Clients")
                {
                    var name = ws.Cells[row, 1].Text;
                    if (!string.IsNullOrWhiteSpace(name)) ExecuteNonQuery("INSERT INTO Clients (Name) VALUES (@Name)", new Dictionary<string, object> { { "@Name", name } });
                }
                else if (type == "Items")
                {
                    var name = ws.Cells[row, 1].Text;
                    var price = ws.Cells[row, 2].GetValue<decimal>();
                    if (!string.IsNullOrWhiteSpace(name)) ExecuteNonQuery("INSERT INTO Items (Name,Price) VALUES (@Name,@Price)", new Dictionary<string, object> { { "@Name", name }, { "@Price", price } });
                }
            }
            LoadAllData();
        }
        #endregion

        #region Helpers
        private int? GetSelectedCategoryId() => (CategoryTreeView.SelectedItem as TreeViewItem)?.Tag as int?;
        #endregion
    }

    // RelayCommand لتسهيل ContextMenu Commands
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) { _execute = execute; _canExecute = canExecute; }
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }
}
