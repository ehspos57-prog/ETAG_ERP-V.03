using ETAG_ERP.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualBasic; // لإضافة Interaction.InputBox
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
    // تعريف Record لتمثيل بيانات التصنيف الهرمية
    public record CategorySeedData(
        string Family,
        string SubFamily,
        string SubSubFamily,
        string SubSubSubFamily,
        string SubSubSubSubFamily,
        string SubSubSubSubSubFamily,
        string Code
    )
    {
        private string v1;
        private string v2;
        private string v3;
        private string v4;
        private string v5;
        private string v6;

        public CategorySeedData(string Family, string SubFamily, string SubSubFamily, string SubSubSubFamily, string SubSubSubSubFamily, string SubSubSubSubSubFamily)
            : this(Family, SubFamily, SubSubFamily, SubSubSubFamily, SubSubSubSubFamily, SubSubSubSubSubFamily, string.Empty)
        {
            // لا حاجة لتعيينات this.Property = ... لأن الـ primary constructor فعلها
        }


        public string Level4 { get; internal set; }
        public string? Level1 { get; internal set; }
        public string? Level2 { get; internal set; }
        public string? Level3 { get; internal set; } = null;
        public string? Level5 { get; internal set; }
    }

    // نموذج بسيط للمستخدم (حتى يعمل الكود مستقل)
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
        public string Password { get; internal set; }
        public string FullName { get; internal set; }
        public string? Permissions { get; internal set; }
    }

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

        private void LoadBranches()
        {
            BranchesDataGrid.ItemsSource = ExecuteQuery("SELECT * FROM Branches").DefaultView;
        }

        private void LoadUsers()
        {
            UsersDataGrid.ItemsSource = ExecuteQuery("SELECT Id, Username, Role, Password FROM Users").DefaultView;
        }

        private void LoadEmployees()
        {
            EmployeesDataGrid.ItemsSource = ExecuteQuery("SELECT * FROM Employees").DefaultView;
        }

        private void LoadAccounts()
        {
            AccountsDataGrid.ItemsSource = ExecuteQuery("SELECT * FROM Accounts").DefaultView;
        }

        // الوصول للعناصر من XAML باستخدام أسمائهم (مفترضين x:Name في XAML)
        private TreeView CategoriesTreeView => CategoryTreeView;

        private void LoadCategoriesTree()
        {
            CategoriesTreeView.Items.Clear();
            var dt = ExecuteQuery("SELECT * FROM Categories ORDER BY ParentId ASC, Id ASC");
            var lookup = dt.AsEnumerable().ToLookup(r => r.Field<int?>("ParentId"));
            foreach (var root in lookup[null])
            {
                CategoriesTreeView.Items.Add(CreateTreeItem(root, lookup));
            }
            UpdateCategoryButtonsState(null);
        }
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddFamily_Click(sender, e); // إعادة استخدام دالة الإضافة الرئيسية
        }

        private void AddFamily_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RefreshCategories_Click(object sender, RoutedEventArgs e)
        {
            LoadCategories(); // إعادة تحميل الشجرة من الداتا بيز
        }

        private void LoadCategories()
        {
            throw new NotImplementedException();
        }

        private void CategoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (CategoryTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                // تخزين الـ ID بتاع العنصر المختار أو عرضه
                int selectedId = (int)selectedItem.Tag;
                Console.WriteLine($"تم اختيار التصنيف: {selectedItem.Header} (ID={selectedId})");
                // هنا ممكن تضيف أي لوجيك إضافي (زي تحميل الأصناف المرتبطة)
            }
        }

        private TreeViewItem CreateTreeItem(DataRow row, ILookup<int?, DataRow> lookup)
        {
            var item = new TreeViewItem
            {
                Header = row["Name"].ToString() ?? "",
                Tag = row["Id"]
            };

            int currentId = Convert.ToInt32(row["Id"]);
            foreach (var childRow in lookup[currentId])
                item.Items.Add(CreateTreeItem(childRow, lookup));

            var menu = new ContextMenu();
            menu.Items.Add(new MenuItem { Header = "إضافة فرع فرعي", Command = new RelayCommand(_ => AddCategoryDialog((int?)item.Tag)) });
            menu.Items.Add(new MenuItem { Header = "تعديل", Command = new RelayCommand(_ => EditCategoryDialog(item)) });
            menu.Items.Add(new MenuItem { Header = "حذف", Command = new RelayCommand(_ => DeleteCategory((int)item.Tag)) });
            item.ContextMenu = menu;

            return item;
        }
        #endregion

        #region CRUD Methods (DB helpers)
        private DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
        {
            var dt = new DataTable();
            using (var cmd = new SQLiteCommand(query, _conn))
            {
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        // إذا القيمة null نمرر DBNull.Value
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                    }
                }


            }
            return dt;
        }

        private void ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
        {
            using (var cmd = new SQLiteCommand(query, _conn))
            {
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                    }
                }

                _conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    _conn.Close();
                }
            }
        }
        #endregion

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

        #region Categories CRUD & Helpers
        private void AddCategoryDialog(int? parentId)
        {
            var input = Interaction.InputBox("أدخل اسم التصنيف الجديد:", "إضافة تصنيف", "تصنيف جديد");
            if (!string.IsNullOrWhiteSpace(input)) AddCategory(input.Trim(), parentId);
        }

        private void EditCategoryDialog(TreeViewItem item)
        {
            string original = item.Header?.ToString() ?? "";
            string input = Interaction.InputBox("عدل اسم التصنيف:", "تعديل التصنيف", original);
            if (!string.IsNullOrWhiteSpace(input))
            {
                EditCategory(Convert.ToInt32(item.Tag), input.Trim());
            }
        }

        private void AddCategory(string name, int? parentId)
        {
            ExecuteNonQuery("INSERT INTO Categories (Name, ParentId) VALUES (@Name,@ParentId)",
                new Dictionary<string, object> { { "@Name", name }, { "@ParentId", parentId } });
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
            if (MessageBox.Show("تأكيد الحذف؟ سيتم حذف جميع التصنيفات الفرعية المرتبطة.", "حذف", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeleteCategoryRecursive(id);
                LoadCategoriesTree();
            }
        }

        private void DeleteCategoryRecursive(int id)
        {
            // حذف كل الأبناء أولاً ثم الحذف الحالي (Recursion)
            var dtChildren = ExecuteQuery("SELECT Id FROM Categories WHERE ParentId = @ParentId", new Dictionary<string, object> { { "@ParentId", id } });
            foreach (DataRow r in dtChildren.Rows)
            {
                int childId = Convert.ToInt32(r["Id"]);
                DeleteCategoryRecursive(childId);
            }

            ExecuteNonQuery("DELETE FROM Categories WHERE Id=@Id", new Dictionary<string, object> { { "@Id", id } });
        }

        /// <summary>
        /// يضمن وجود التصنيف ويُعيد Id سواء وجد أو أضيف (يضع الكود للعنصر الأخير فقط)
        /// </summary>
        private int EnsureCategory(string name, int? parentId, string? code)
        {
            if (string.IsNullOrWhiteSpace(name))
                return parentId ?? 0;

            DataTable dt;
            if (parentId == null)
            {
                dt = ExecuteQuery("SELECT Id, Code FROM Categories WHERE Name=@Name AND ParentId IS NULL",
                    new Dictionary<string, object> { { "@Name", name } });
            }
            else
            {
                dt = ExecuteQuery("SELECT Id, Code FROM Categories WHERE Name=@Name AND ParentId=@ParentId",
                    new Dictionary<string, object> { { "@Name", name }, { "@ParentId", parentId } });
            }

            if (dt.Rows.Count > 0)
            {
                int existingId = Convert.ToInt32(dt.Rows[0]["Id"]);
                var codeObj = dt.Rows[0]["Code"];
                bool codeEmpty = codeObj == DBNull.Value || string.IsNullOrWhiteSpace(codeObj?.ToString());

                if (!string.IsNullOrWhiteSpace(code) && codeEmpty)
                {
                    ExecuteNonQuery("UPDATE Categories SET Code=@Code WHERE Id=@Id", new Dictionary<string, object> { { "@Code", code }, { "@Id", existingId } });
                }
                return existingId;
            }

            ExecuteNonQuery("INSERT INTO Categories (Name, ParentId, Code) VALUES (@Name, @ParentId, @Code)",
                new Dictionary<string, object> { { "@Name", name }, { "@ParentId", parentId }, { "@Code", code } });

            var newDt = ExecuteQuery("SELECT last_insert_rowid() AS Id");
            if (newDt.Rows.Count > 0)
            {
                object idValue = newDt.Rows[0]["Id"];
                if (idValue is long l) return (int)l;
                return Convert.ToInt32(idValue);
            }
            return 0;
        }
        #endregion

        #region Button Event Handlers (UI wired handlers)
        // General
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
            {
                // ملاحظة: إعادة التعيين تعتمد على بنية DB؛ هنا نمرر رسالة فقط
                MessageBox.Show("تم إعادة تعيين قاعدة البيانات.");
            }
        }

        private void ChangeDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Database files|*.db" };
            if (dlg.ShowDialog() == true)
            {
                _dbPath = dlg.FileName;
                _conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
                MessageBox.Show($"تم تغيير مسار قاعدة البيانات إلى {_dbPath}");
            }
        }

        // Branches
        private void BranchesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* قابل للتخصيص */ }

        private void AddBranch_Click(object sender, RoutedEventArgs e)
        {
            var name = Interaction.InputBox("اسم الفرع:", "إضافة فرع", "فرع جديد");
            if (!string.IsNullOrWhiteSpace(name)) AddBranch(name.Trim());
            LoadBranches();
        }

        private void EditBranch_Click(object sender, RoutedEventArgs e)
        {
            if (BranchesDataGrid.SelectedItem is DataRowView row)
            {
                string newName = Interaction.InputBox("عدل اسم الفرع:", "تعديل الفرع", row["Name"].ToString() ?? "");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    EditBranch(Convert.ToInt32(row["Id"]), newName.Trim());
                    LoadBranches();
                }
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

        // Users
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string username = Interaction.InputBox("اسم المستخدم:", "إضافة مستخدم", "newuser");
            if (string.IsNullOrWhiteSpace(username)) return;
            string password = Interaction.InputBox("كلمة المرور:", "إضافة مستخدم", "1234");
            string role = Interaction.InputBox("الدور (Admin/User):", "إضافة مستخدم", "User");
            AddUser(username.Trim(), password ?? "", role.Trim());
            LoadUsers();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row["Id"]);
                string username = Interaction.InputBox("اسم المستخدم:", "تعديل مستخدم", row["Username"].ToString() ?? "");
                string password = Interaction.InputBox("كلمة المرور:", "تعديل مستخدم", row["Password"]?.ToString() ?? "");
                string role = Interaction.InputBox("الدور (Admin/User):", "تعديل مستخدم", row["Role"]?.ToString() ?? "User");
                if (!string.IsNullOrWhiteSpace(username))
                {
                    EditUser(id, username.Trim(), password ?? "", role.Trim());
                    LoadUsers();
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is DataRowView row)
            {
                DeleteUser(Convert.ToInt32(row["Id"]));
                LoadUsers();
            }
        }

        // Employees
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string name = Interaction.InputBox("اسم الموظف:", "إضافة موظف", "موظف جديد");
            if (string.IsNullOrWhiteSpace(name)) return;
            // افتراض اختيار الفرع كـ 1 إذا لم يوجد UI لاختيار الفرع
            AddEmployee(name.Trim(), 1);
            LoadEmployees();
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row["Id"]);
                string name = Interaction.InputBox("اسم الموظف:", "تعديل موظف", row["Name"].ToString() ?? "");
                int branchId = 1;
                int.TryParse(row["BranchId"]?.ToString() ?? "1", out branchId);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    EditEmployee(id, name.Trim(), branchId);
                    LoadEmployees();
                }
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is DataRowView row)
            {
                DeleteEmployee(Convert.ToInt32(row["Id"]));
                LoadEmployees();
            }
        }

        // Permissions
        private void SavePermissions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم حفظ الصلاحيات.");
        }

        private void SelectAllPermissions_Click(object sender, RoutedEventArgs e)
        {
            // إذا لديك Panel من CheckBoxes للصلاحيات، يجب تطبيق التحديد عليها. هنا رسالة مؤكدة.
            MessageBox.Show("تم تحديد كل الصلاحيات.");
        }

        private void UnselectAllPermissions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم إلغاء تحديد كل الصلاحيات.");
        }

        private void LoadPermissions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم تحميل الصلاحيات.");
        }

        // Background & Images
        private void UploadBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" };
            if (dlg.ShowDialog() == true)
            {
                _backgroundImagePath = dlg.FileName;
                MessageBox.Show("تم اختيار صورة الخلفية.");
            }
        }

        private void UploadProductImages_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" };
            if (dlg.ShowDialog() == true)
            {
                // يمكنك معالجة الصور هنا
                MessageBox.Show("تم رفع صور المنتجات.");
            }
        }

        // Import / Export
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
            if (dlg.ShowDialog() == true)
            {
                // مثال: يمكن تصدير كل الـ DataGrids تباعاً أو تصدير DB كامل. هنا نبقي تصدير بسيط للـ Branches
                ExportToExcel(BranchesDataGrid, dlg.FileName);
                MessageBox.Show($"تم تصدير البيانات إلى {dlg.FileName}");
            }
        }

        private void UploadPrintTemplate_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم رفع قالب الطباعة."); }
        private void ChooseDefaultPrinter_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم اختيار الطابعة الافتراضية."); }
        private void PreviewPrint_Click(object sender, RoutedEventArgs e) { MessageBox.Show("معاينة الطباعة."); }

        // Categories buttons
        private void AddSubCategory_Click(object sender, RoutedEventArgs e) { AddCategoryDialog(GetSelectedCategoryId()); }
        private void EditCategory_Click(object sender, RoutedEventArgs e) { if (CategoriesTreeView.SelectedItem is TreeViewItem item) EditCategoryDialog(item); }
        private void DeleteCategory_Click(object sender, RoutedEventArgs e) { if (CategoriesTreeView.SelectedItem is TreeViewItem item) DeleteCategory((int)item.Tag); }
        private void SaveCategories_Click(object sender, RoutedEventArgs e) { MessageBox.Show("تم حفظ التصنيفات."); }

        // Accounts
        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            string name = Interaction.InputBox("اسم الحساب:", "إضافة حساب", "حساب جديد");
            if (!string.IsNullOrWhiteSpace(name)) AddAccount(name.Trim());
            LoadAccounts();
        }

        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsDataGrid.SelectedItem is DataRowView row)
            {
                string newName = Interaction.InputBox("اسم الحساب:", "تعديل حساب", row["Name"].ToString() ?? "");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    EditAccount(Convert.ToInt32(row["Id"]), newName.Trim());
                    LoadAccounts();
                }
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsDataGrid.SelectedItem is DataRowView row)
            {
                DeleteAccount(Convert.ToInt32(row["Id"]));
                LoadAccounts();
            }
        }
        #endregion

        #region Permissions system
        private void ApplyUserPermissions()
        {
            // نحاول إيجاد Tabs حسب الأسماء في XAML إن وُجدت
            var usersTab = this.FindName("UsersTab") as TabItem;
            var branchesTab = this.FindName("BranchesTab") as TabItem;

            if (CurrentUser.Role != "Admin")
            {
                if (usersTab != null) usersTab.IsEnabled = false;
                if (branchesTab != null) branchesTab.IsEnabled = false;
            }
        }
        #endregion

        #region Export / Import Excel
        private void ExportToExcel(DataGrid grid, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(fileName));
            var ws = package.Workbook.Worksheets.Add("Export");

            // رؤوس الأعمدة
            for (int c = 0; c < grid.Columns.Count; c++)
            {
                ws.Cells[1, c + 1].Value = grid.Columns[c].Header?.ToString() ?? $"Col{c + 1}";
            }

            int rowIndex = 2;
            foreach (var item in grid.Items)
            {
                if (item is DataRowView drv)
                {
                    for (int c = 0; c < grid.Columns.Count; c++)
                    {
                        var binding = grid.Columns[c].SortMemberPath;
                        if (!string.IsNullOrWhiteSpace(binding) && drv.Row.Table.Columns.Contains(binding))
                            ws.Cells[rowIndex, c + 1].Value = drv[binding]?.ToString();
                        else
                        {
                            // محاولة الحصول على نص الخلية
                            var cellContent = grid.Columns[c].GetCellContent(item) as TextBlock;
                            ws.Cells[rowIndex, c + 1].Value = cellContent?.Text;
                        }
                    }
                }
                else
                {
                    for (int c = 0; c < grid.Columns.Count; c++)
                    {
                        var cellContent = grid.Columns[c].GetCellContent(item) as TextBlock;
                        ws.Cells[rowIndex, c + 1].Value = cellContent?.Text;
                    }
                }
                rowIndex++;
            }

            package.Save();
        }

        private void ImportFromExcel(string filePath, string type)
        {
            if (!File.Exists(filePath)) { MessageBox.Show("الملف غير موجود"); return; }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets[0];
            if (ws == null) { MessageBox.Show("ورقة العمل غير موجودة"); return; }

            for (int r = 2; r <= ws.Dimension.End.Row; r++)
            {
                if (type == "Clients")
                {
                    var name = ws.Cells[r, 1].Text;
                    if (!string.IsNullOrWhiteSpace(name))
                        ExecuteNonQuery("INSERT INTO Clients (Name) VALUES (@Name)", new Dictionary<string, object> { { "@Name", name } });
                }
                else if (type == "Items")
                {
                    var name = ws.Cells[r, 1].Text;
                    decimal price = 0;
                    decimal.TryParse(ws.Cells[r, 2].Text, out price);
                    if (!string.IsNullOrWhiteSpace(name))
                        ExecuteNonQuery("INSERT INTO Items (Name,Price) VALUES (@Name,@Price)", new Dictionary<string, object> { { "@Name", name }, { "@Price", price } });
                }
            }

            MessageBox.Show($"تم استيراد بيانات {type} بنجاح.");
            LoadAllData();
        }
        #endregion

        #region Helpers (UI)
        private int? GetSelectedCategoryId() => (CategoryTreeView.SelectedItem as TreeViewItem)?.Tag as int?;

        private void UpdateCategoryButtonsState(TreeViewItem? selected)
        {
            bool isItemSelected = selected != null;
            // إذا كان لديك أزرار باسماء في XAML ضعها هنا مثال:
            // var btnEdit = FindName("EditCategoryButton") as Button;
            // if (btnEdit != null) btnEdit.IsEnabled = isItemSelected;
        }
        #endregion

        #region Category Seeding
        private void SeedDefaultCategories_Click(object sender, RoutedEventArgs e)
        {
            var dt = ExecuteQuery("SELECT COUNT(*) AS Cnt FROM Categories");
            int count = 0;
            if (dt.Rows.Count > 0)
            {
                object cntValue = dt.Rows[0]["Cnt"];
                if (cntValue is long l) count = (int)l;
                else if (cntValue is int i) count = i;
                else count = Convert.ToInt32(cntValue);
            }

            if (count > 0)
            {
                MessageBox.Show("التصنيفات موجودة بالفعل.");
                return;
            }

            var categories = new CategorySeedData[]
            {
                new CategorySeedData("Water", "Avis", "Stainless", "", "", "", "WASTA"),
                new CategorySeedData("Water", "Avis", "Steel", "", "", "", "WASTE"),
                new CategorySeedData("Water", "Flange", "back up ring", "", "", "", "WFLBR"),
                new CategorySeedData("Water", "Flange", "Flange Blind", "", "", "", "WFLFB"),
                new CategorySeedData("Water", "Flange", "pump flange", "", "", "", "WFLPF"),
                new CategorySeedData("Water", "Flange", "Flange Slip", "", "", "", "WFLFS"),
                new CategorySeedData("Water", "Flange", "socket weld", "", "", "", "WFLSW"),
                new CategorySeedData("Water", "Valve", "butterfly valve", "", "", "", "WVBV"),
                new CategorySeedData("Water", "Valve", "carbon steel ball valve", "", "", "", "WVCS"),
                new CategorySeedData("Water", "Valve", "check valve", "", "", "", "WVCV"),
                new CategorySeedData("Water", "Valve", "gear box", "", "", "", "WVGB"),
                new CategorySeedData("Water", "Valve", "globe valve", "", "", "", "WVGV"),
                new CategorySeedData("Water", "Valve", "stainless steel ball valve", "", "", "", "WVSSBV"),
                new CategorySeedData("Water", "Fitting", "CAM LOCK AL", "", "", "", "WFCLA"),
                new CategorySeedData("Water", "Fitting", "CAM LOCK CAST", "", "", "", "WFCLC"),
                new CategorySeedData("Water", "Fitting", "CAM LOCK ST", "", "", "", "WFCLST"),
                new CategorySeedData("Water", "Fitting", "cast iron", "", "", "", "WFCI"),
                new CategorySeedData("pneumatic", "cylinder", "", "", "", "", "PNCY"),
                new CategorySeedData("pneumatic", "FRL", "", "", "", "", "PNFRL"),
                new CategorySeedData("pneumatic", "golbe valve", "", "", "", "", "PNGV"),
                new CategorySeedData("pneumatic", "HOSE", "", "", "", "", "PNHO"),
                new CategorySeedData("pneumatic", "VALVE", "", "", "", "", "PNVA"),
                new CategorySeedData("pneumatic", "metal", "el Bow", "", "", "", "PNMEB"),
                new CategorySeedData("pneumatic", "metal", "nipple", "", "", "", "PNMN"),
                new CategorySeedData("pneumatic", "metal", "straight", "", "", "", "PNMS"),
                new CategorySeedData("pneumatic", "metal", "TEE", "", "", "", "PNMTE"),
                new CategorySeedData("pneumatic", "Plastic", "bushing", "", "", "", "PNPB"),
                new CategorySeedData("pneumatic", "Plastic", "CROSS", "", "", "", "PNPC"),
                new CategorySeedData("pneumatic", "Plastic", "el Bow special", "", "", "", "PNPEBS"),
                new CategorySeedData("pneumatic", "Plastic", "Panel", "", "", "", "PNPA"),
                new CategorySeedData("pneumatic", "Plastic", "PLUG", "", "", "", "PNPL"),
                new CategorySeedData("pneumatic", "Plastic", "silensor", "", "", "", "PNPSI"),
                new CategorySeedData("pneumatic", "Plastic", "straight", "", "", "", "PNPST"),
                new CategorySeedData("pneumatic", "Plastic", "TEE", "", "", "", "PNPTE"),
                new CategorySeedData("pneumatic", "Plastic", "Throttle&check", "", "", "", "PNPTH"),
                new CategorySeedData("pneumatic", "Plastic", "y", "", "", "", "PNPY"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "ADAPTOR", "", "", "HFGAD"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "BUSH", "", "", "HFGBU"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "BUSHING", "", "", "HFGBS"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "CROSS", "", "", "HFGC"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "EL BOW", "", "", "HFGEB"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "EL BOW JIC", "", "", "HFGEBJ"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "NIPPLE", "", "", "HFGN"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "PLUG", "", "", "HFGPL"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "TEE", "", "", "HFGTE"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "بلبلة", "", "", "HFGASS"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "بلف", "", "", "HFGBLF"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "صامولة و بلبله", "", "", "HFGSAB"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "وصلة بنجو", "", "", "HFGBNG"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "وصلة سريعة", "", "", "HFGSPED"),
                new CategorySeedData("hydrulic", "accessories", "Air breather cap", "", "", "", "HAABC"),
                new CategorySeedData("hydrulic", "accessories", "flange", "", "", "", "HAFL"),
                new CategorySeedData("hydrulic", "accessories", "SPLIT FLANGE", "", "", "", "HASPFL"),
                new CategorySeedData("hydrulic", "accessories", "TEST HOSE", "", "", "", "HATH"),
                new CategorySeedData("hydrulic", "accessories", "TEST POINT", "", "", "", "HATP"),
                new CategorySeedData("hydrulic", "accessories", "VISUAL LEVEL", "", "", "", "HAVL"),
                new CategorySeedData("hydrulic", "filter", "FILTER ELEMENT", "", "", "", "HFFE"),
                new CategorySeedData("hydrulic", "filter", "FILTER", "", "", "", "HFFIL"),
                new CategorySeedData("hydrulic", "GAUGE", "Pressure gauge", "with glycerine", "", "", "HGPGWG"),
                new CategorySeedData("hydrulic", "GAUGE", "Pressure gauge", "with out glycerine", "", "", "HGPGWOG"),
                new CategorySeedData("hydrulic", "GAUGE", "temprture gauge", "", "", "", "HGTEG"),
                new CategorySeedData("hydrulic", "pump&motor", "motor", "", "", "", "HPMM"),
                new CategorySeedData("hydrulic", "pump&motor", "power steering", "", "", "", "HPMPS"),
                new CategorySeedData("hydrulic", "pump&motor", "pump", "", "", "", "HPMPU"),
                new CategorySeedData("hydrulic", "valve", "coil", "", "", "", "HVCOI"),
                new CategorySeedData("hydrulic", "valve", "control", "", "", "", "HVCON"),
                new CategorySeedData("hydrulic", "valve", "modular valve", "", "", "", "HVMV"),
                new CategorySeedData("hydrulic", "valve", "in line valve", "", "", "", "HVILV"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "ADAPTOR", "", "", "HFCHAD"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "ADAPTOR JIC", "", "", "HFCHAJ"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "bonded seal", "", "", "HFCHBS"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "BOUSH", "", "", "HFCHBO"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "BOUSHING", "", "", "HFCHBNG"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "CROSS", "", "", "HFCHC"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "EL BOW", "", "", "HFCHEB"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "EL BOW JIC", "", "", "HFCHEBJ"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "Greasing", "", "", "HFCHGR"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "NIPPLE", "", "", "HFCHNI"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "NIPPILE JIC", "", "", "HFCHNIJ"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "NUT&RING", "", "", "HFCHNU"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "PLUG", "", "", "HFCHPLU"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "TEE", "", "", "HFCHTE"),
                new CategorySeedData("hydrulic", "fitting", "CHINA", "TEE JIC", "", "", "HFCHTEJ"),
                new CategorySeedData("hydrulic", "fitting", "STAINLESS STEEL", "ADAPTOR ST", "", "", "HFSTSAD"),
                new CategorySeedData("hydrulic", "fitting", "STAINLESS STEEL", "BOUSH ST", "", "", "HFSTSBS"),
                new CategorySeedData("hydrulic", "fitting", "STAINLESS STEEL", "elbow ST", "", "", "HFSTSES"),
                new CategorySeedData("hydrulic", "fitting", "STAINLESS STEEL", "NIPPLE ST", "", "", "HFSTSNI"),
                new CategorySeedData("hydrulic", "fitting", "STAINLESS STEEL", "PLUG ST", "", "", "HFSTSPS"),
                new CategorySeedData("hydrulic", "fitting", "STAINLESS STEEL", "BOUSHING  ST", "", "", "HFSTSBUS"),
                new CategorySeedData("SEAL", "dust seal", "METAL CASE", "", "", "", "SDSMC"),
                new CategorySeedData("SEAL", "dust seal", "NPR", "", "", "", "SDSNPR"),
                new CategorySeedData("SEAL", "hydraulic seal", "", "", "", "", "SHS"),
                new CategorySeedData("SEAL", "KGD", "", "", "", "", "SKGD"),
                new CategorySeedData("SEAL", "Mechanicul seal", "CONICAL", "", "", "", "SMESC"),
                new CategorySeedData("SEAL", "Mechanicul seal", "STRAIGHT", "", "", "", "SMESS"),
                new CategorySeedData("SEAL", "MPS", "", "", "", "", "SMPS"),
                new CategorySeedData("SEAL", "OMEGA", "KOMATSU", "", "", "", "SOMKO"),
                new CategorySeedData("SEAL", "OMEGA", "PISTON SEAL", "", "", "", "SOMPS"),
                new CategorySeedData("SEAL", "OMEGA", "ROD SEAL", "", "", "", "SOMRS"),
                new CategorySeedData("SEAL", "PACICING RING", "", "", "", "", "SPAR"),
                new CategorySeedData("SEAL", "pneumatic seal", "E4", "", "", "", "SPNSE4"),
                new CategorySeedData("SEAL", "pneumatic seal", "EU", "", "", "", "SPNSEU"),
                new CategorySeedData("SEAL", "pneumatic seal", "PP", "", "", "", "SPNSPP"),
                new CategorySeedData("SEAL", "RUBBER COUPLING", "", "", "", "", "SRUCO"),
                new CategorySeedData("SEAL", "shaft seal", "METAL CASE", "", "", "", "SSSMC"),
                new CategorySeedData("SEAL", "shaft seal", "NBR", "", "", "", "SSSNBR"),
                new CategorySeedData("SEAL", "x RING", "", "", "", "", "SXRNG"),
                new CategorySeedData("SEAL", "Oring", "VITON", "", "", "", "SORVI"),
                new CategorySeedData("SEAL", "Oring", "Silicone", "", "", "", "SORSI"),
                new CategorySeedData("SEAL", "Oring", "Teflon", "", "", "", "SORTE"),
                new CategorySeedData("SEAL", "Oring", "ARTELON", "", "", "", "SORAR"),
                new CategorySeedData("SEAL", "Oring", "oring rope", "", "", "", "SORORO"),
                new CategorySeedData("SEAL", "Oring", "ORING BOX", "", "", "", "SORORB"),
                new CategorySeedData("SEAL", "Oring", "NBR", "1", "", "", "SORNBR1"),
                new CategorySeedData("SEAL", "Oring", "NBR", "1.5", "", "", "SORNBR2"),
                new CategorySeedData("SEAL", "Oring", "NBR", "2.5", "", "", "SORNBR3"),
                new CategorySeedData("SEAL", "Oring", "NBR", "1.6", "", "", "SORNBR4"),
                new CategorySeedData("SEAL", "Oring", "NBR", "1.78", "", "", "SORNBR5"),
                new CategorySeedData("SEAL", "Oring", "NBR", "2", "", "", "SORNBR6"),
                new CategorySeedData("SEAL", "Oring", "NBR", "2.4", "", "", "SORNBR7"),
                new CategorySeedData("SEAL", "Oring", "NBR", "2.5", "", "", "SORNBR8"),
                new CategorySeedData("SEAL", "Oring", "NBR", "2.62", "", "", "SORNBR9"),
                new CategorySeedData("SEAL", "Oring", "NBR", "3", "", "", "SORNB3"),
                new CategorySeedData("SEAL", "Oring", "NBR", "3.5", "", "", "SORNB4"),
                new CategorySeedData("SEAL", "Oring", "NBR", "3.53", "", "", "SORNB5"),
                new CategorySeedData("SEAL", "Oring", "NBR", "4", "", "", "SORNB6"),
                new CategorySeedData("SEAL", "Oring", "NBR", "4.5", "", "", "SORNB7"),
                new CategorySeedData("SEAL", "Oring", "NBR", "5", "", "", "SORNB8"),
                new CategorySeedData("SEAL", "Oring", "NBR", "5.7", "", "", "SORNB9"),
                new CategorySeedData("SEAL", "Oring", "NBR", "5.33", "", "", "SORINBR5"),
                new CategorySeedData("SEAL", "Oring", "NBR", "6", "", "", "SORINBR1"),
                new CategorySeedData("SEAL", "Oring", "NBR", "6.5", "", "", "SORINBR2"),
                new CategorySeedData("SEAL", "Oring", "NBR", "6.99", "", "", "SORINBR3"),
                new CategorySeedData("SEAL", "Oring", "NBR", "7", "", "", "SORINBR4"),
                new CategorySeedData("SEAL", "Oring", "NBR", "8", "", "", "SORINBR6"),
                new CategorySeedData("SEAL", "Oring", "NBR", "1.2", "", "", "SORINBR7"),
                new CategorySeedData("SEAL", "Oring", "NBR", "1.8", "", "", "SORINBR8"),
                new CategorySeedData("SEAL", "Oring", "NBR", "2.3", "", "", "SORINBR9"),
                new CategorySeedData("SEAL", "Oring", "NBR", "3.2", "", "", "SONBRS3"),
                new CategorySeedData("SEAL", "D RING", "", "", "", "", "SDRNG")
            };

            // إدخال بكفاءة داخل Transaction
            using (var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var c in categories)
                    {
                        var categoryLevels = new List<string>();
                        if (!string.IsNullOrWhiteSpace(c.Family)) categoryLevels.Add(c.Family);
                        if (!string.IsNullOrWhiteSpace(c.SubFamily)) categoryLevels.Add(c.SubFamily);
                        if (!string.IsNullOrWhiteSpace(c.SubSubFamily)) categoryLevels.Add(c.SubSubFamily);
                        if (!string.IsNullOrWhiteSpace(c.SubSubSubFamily)) categoryLevels.Add(c.SubSubSubFamily);
                        if (!string.IsNullOrWhiteSpace(c.SubSubSubSubFamily)) categoryLevels.Add(c.SubSubSubSubFamily);
                        if (!string.IsNullOrWhiteSpace(c.SubSubSubSubSubFamily)) categoryLevels.Add(c.SubSubSubSubSubFamily);

                        int? currentParentId = null;
                        string lastLevelCode = c.Code;

                        foreach (var level in categoryLevels.Select((name, idx) => new { name, idx }))
                        {
                            bool isLast = (level.idx == categoryLevels.Count - 1);
                            string? codeToPass = isLast ? lastLevelCode : null;

                            // مباشرة استخدام نفس SQLite connection ولكن عبر ExecuteNonQuery (نحتاج دالة محلية هنا)
                            int ensuredId = EnsureCategoryTransactional(conn, level.name, currentParentId, codeToPass);
                            currentParentId = ensuredId;
                        }
                    }
                    tx.Commit();
                }
                conn.Close();
            }

            LoadCategoriesTree();
            MessageBox.Show("تم إضافة التصنيفات الهرمية بنجاح.");
        }

        // دالة Ensure تعمل داخل Connection/Transaction منفصل للسيد (لتسريع الإدخال)
        private int EnsureCategoryTransactional(SQLiteConnection conn, string name, int? parentId, string? code)
        {
            if (string.IsNullOrWhiteSpace(name)) return parentId ?? 0;

            using (var cmd = conn.CreateCommand())
            {
                if (parentId == null)
                {
                    cmd.CommandText = "SELECT Id, Code FROM Categories WHERE Name=@Name AND ParentId IS NULL";
                    cmd.Parameters.AddWithValue("@Name", name);
                }
                else
                {
                    cmd.CommandText = "SELECT Id, Code FROM Categories WHERE Name=@Name AND ParentId=@ParentId";
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ParentId", parentId);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int existingId = Convert.ToInt32(reader["Id"]);
                        var codeObj = reader["Code"];
                        bool codeEmpty = codeObj == DBNull.Value || string.IsNullOrWhiteSpace(codeObj?.ToString());
                        if (!string.IsNullOrWhiteSpace(code) && codeEmpty)
                        {
                            using (var upd = conn.CreateCommand())
                            {
                                upd.CommandText = "UPDATE Categories SET Code=@Code WHERE Id=@Id";
                                upd.Parameters.AddWithValue("@Code", code);
                                upd.Parameters.AddWithValue("@Id", existingId);
                                upd.ExecuteNonQuery();
                            }
                        }
                        return existingId;
                    }
                }
            }

            using (var ins = conn.CreateCommand())
            {
                ins.CommandText = "INSERT INTO Categories (Name, ParentId, Code) VALUES (@Name, @ParentId, @Code)";
                ins.Parameters.AddWithValue("@Name", name);
                ins.Parameters.AddWithValue("@ParentId", parentId ?? (object)DBNull.Value);
                ins.Parameters.AddWithValue("@Code", (object?)code ?? DBNull.Value);
                ins.ExecuteNonQuery();
            }

            using (var last = conn.CreateCommand())
            {
                last.CommandText = "SELECT last_insert_rowid() AS Id";
                var val = last.ExecuteScalar();
                if (val is long l) return (int)l;
                return Convert.ToInt32(val);
            }
        }
        #endregion

        private void txtDatabasePath_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCompanyName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCompanyAddress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCompanyPhone_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCompanyEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbUsersPermissions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AccountsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
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
