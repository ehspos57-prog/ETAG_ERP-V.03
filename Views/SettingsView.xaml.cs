using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Category = ETAG_ERP.Models.Category;

namespace ETAG_ERP.Views
{
    public partial class SettingsView : Window
    {
        // Collections
        public ObservableCollection<Branch> Branches { get; set; } = new ObservableCollection<Branch>();
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
        public ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();


        public SettingsView()
        {
            InitializeComponent();

            // Bind DataGrids
            BranchesDataGrid.ItemsSource = Branches;
            UsersDataGrid.ItemsSource = Users;
            EmployeesDataGrid.ItemsSource = Employees;
            AccountsDataGrid.ItemsSource = Accounts;

            // Load initial data from DB
            LoadBranches();
            LoadUsers();
            LoadEmployees();
            LoadAccounts();

            LoadCategories();
        }

        #region General Settings - Database

        private void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("هل أنت متأكد من إعادة إنشاء قاعدة البيانات؟ سيتم حذف جميع البيانات.",
                                         "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                DatabaseHelper.ResetDatabase();
                MessageBox.Show("تم إعادة إنشاء قاعدة البيانات.");
                LoadAll();
            }
        }

        private void ChangeDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Database files (*.db)|*.db|All files (*.*)|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                txtDatabasePath.Text = ofd.FileName;
            }
        }

        private void SaveGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper.ExecuteNonQuery($"UPDATE Settings SET CompanyName='{txtCompanyName.Text}', " +
                                           $"Address='{txtCompanyAddress.Text}', Phone='{txtCompanyPhone.Text}', Email='{txtCompanyEmail.Text}'");
            MessageBox.Show("تم حفظ الإعدادات العامة.");
        }

        #endregion

        #region Branches CRUD

        private void LoadBranches()
        {
            Branches.Clear();
            DataTable dt = DatabaseHelper.GetDataTable("SELECT * FROM Branches");
            foreach (DataRow row in dt.Rows)
            {
                Branches.Add(new Branch
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Address = row["Address"].ToString(),
                    Phone = row["Phone"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }
        }

        private void AddBranch_Click(object sender, RoutedEventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("اسم الفرع الجديد:", "إضافة فرع", "فرع جديد");
            if (!string.IsNullOrWhiteSpace(name))
            {
                DatabaseHelper.ExecuteNonQuery($"INSERT INTO Branches(Name, Address, Phone, IsActive) VALUES('{name}', '', '', 1)");
                LoadBranches();
            }
        }

        private void EditBranch_Click(object sender, RoutedEventArgs e)
        {
            if (BranchesDataGrid.SelectedItem is Branch branch)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("تعديل اسم الفرع:", "تعديل فرع", branch.Name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    DatabaseHelper.ExecuteNonQuery($"UPDATE Branches SET Name='{newName}' WHERE Id={branch.Id}");
                    LoadBranches();
                }
            }
        }

        private void DeleteBranch_Click(object sender, RoutedEventArgs e)
        {
            if (BranchesDataGrid.SelectedItem is Branch branch)
            {
                var result = MessageBox.Show("هل تريد حذف هذا الفرع؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DatabaseHelper.ExecuteNonQuery($"DELETE FROM Branches WHERE Id={branch.Id}");
                    LoadBranches();
                }
            }
        }

        private void BranchesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        #endregion

        #region Users CRUD

        private void LoadUsers()
        {
            Users.Clear();
            DataTable dt = DatabaseHelper.GetDataTable("SELECT * FROM Users");
            foreach (DataRow row in dt.Rows)
            {
                Users.Add(new User
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Role = row["Role"].ToString(),
                    IsAdmin = Convert.ToBoolean(row["IsAdmin"])
                });
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string username = Microsoft.VisualBasic.Interaction.InputBox("اسم المستخدم:", "إضافة مستخدم", "user");
            if (!string.IsNullOrWhiteSpace(username))
            {
                DatabaseHelper.ExecuteNonQuery($"INSERT INTO Users(Username, FullName, Role, IsAdmin) VALUES('{username}', '', '', 0)");
                LoadUsers();
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User user)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("الاسم الكامل:", "تعديل المستخدم", user.FullName);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    DatabaseHelper.ExecuteNonQuery($"UPDATE Users SET FullName='{newName}' WHERE Id={user.Id}");
                    LoadUsers();
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User user)
            {
                var result = MessageBox.Show("هل تريد حذف هذا المستخدم؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DatabaseHelper.ExecuteNonQuery($"DELETE FROM Users WHERE Id={user.Id}");
                    LoadUsers();
                }
            }
        }

        #endregion

        #region Employees CRUD

        private void LoadEmployees()
        {
            Employees.Clear();
            DataTable dt = DatabaseHelper.GetDataTable("SELECT * FROM Employees");
            foreach (DataRow row in dt.Rows)
            {
                Employees.Add(new Employee
                {
                    Id = Convert.ToInt32(row["Id"]),
                    FullName = row["FullName"].ToString(),
                    JobTitle = row["JobTitle"].ToString(),
                    Phone = row["Phone"].ToString()
                });
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("اسم الموظف:", "إضافة موظف", "موظف جديد");
            if (!string.IsNullOrWhiteSpace(name))
            {
                DatabaseHelper.ExecuteNonQuery($"INSERT INTO Employees(FullName, JobTitle, Phone) VALUES('{name}', '', '')");
                LoadEmployees();
            }
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee emp)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("الاسم الكامل:", "تعديل الموظف", emp.FullName);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    DatabaseHelper.ExecuteNonQuery($"UPDATE Employees SET FullName='{newName}' WHERE Id={emp.Id}");
                    LoadEmployees();
                }
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee emp)
            {
                var result = MessageBox.Show("هل تريد حذف هذا الموظف؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DatabaseHelper.ExecuteNonQuery($"DELETE FROM Employees WHERE Id={emp.Id}");
                    LoadEmployees();
                }
            }
        }

        #endregion

        #region Accounts CRUD

        private void LoadAccounts()
        {
            Accounts.Clear();
            DataTable dt = DatabaseHelper.GetDataTable("SELECT * FROM Accounts");
            foreach (DataRow row in dt.Rows)
            {
                Accounts.Add(new Account
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Type = row["Type"].ToString(),
                    Balance = Convert.ToDecimal(row["Balance"])
                });
            }
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("اسم الحساب:", "إضافة حساب", "حساب جديد");
            if (!string.IsNullOrWhiteSpace(name))
            {
                DatabaseHelper.ExecuteNonQuery($"INSERT INTO Accounts(Name, Type, Balance) VALUES('{name}', '', 0)");
                LoadAccounts();
            }
        }

        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsDataGrid.SelectedItem is Account acc)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("اسم الحساب:", "تعديل الحساب", acc.Name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    DatabaseHelper.ExecuteNonQuery($"UPDATE Accounts SET Name='{newName}' WHERE Id={acc.Id}");
                    LoadAccounts();
                }
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsDataGrid.SelectedItem is Account acc)
            {
                var result = MessageBox.Show("هل تريد حذف هذا الحساب؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DatabaseHelper.ExecuteNonQuery($"DELETE FROM Accounts WHERE Id={acc.Id}");
                    LoadAccounts();
                }
            }
        }

        private void SaveAccountsSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم حفظ إعدادات الحسابات والخزنة.");
        }

        #endregion

        #region Categories CRUD

        private void LoadCategories()
        {
            Categories.Clear();
            DataTable dt = DatabaseHelper.GetDataTable("SELECT * FROM Categories ORDER BY Id");
            foreach (DataRow row in dt.Rows)
            {
                Categories.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    ParentId = row["ParentId"] != DBNull.Value ? Convert.ToInt32(row["ParentId"]) : 0,
                    Type = row["Type"].ToString()
                });
            }

            BuildCategoryTree();
        }

        private void BuildCategoryTree()
        {
            CategoryTreeView.Items.Clear();
            foreach (var cat in Categories)
            {
                if (cat.ParentId == null)
                {
                    TreeViewItem item = CreateTreeItem(cat);
                    CategoryTreeView.Items.Add(item);
                    AddChildCategories(item, cat.Id);
                }
            }
        }

        private TreeViewItem CreateTreeItem(Category cat)
        {
            TreeViewItem item = new TreeViewItem
            {
                Header = cat.Name,
                Tag = cat.Id
            };
            return item;
        }

        private void AddChildCategories(TreeViewItem parentItem, int parentId)
        {
            foreach (var child in Categories)
            {
                if (child.ParentId == parentId)
                {
                    TreeViewItem childItem = CreateTreeItem(child);
                    parentItem.Items.Add(childItem);
                    AddChildCategories(childItem, child.Id);
                }
            }
        }

        private int? GetSelectedCategoryId()
        {
            if (CategoryTreeView.SelectedItem is TreeViewItem item)
                return (int)item.Tag;
            return null;
        }

        private void AddCategory(int? parentId, string type)
        {
            string name = "تصنيف جديد";
            DatabaseHelper.ExecuteNonQuery($"INSERT INTO Categories(Name, ParentId, Type) VALUES('{name}', {(parentId.HasValue ? parentId.Value.ToString() : "NULL")}, '{type}')");
            LoadCategories();
        }

        private void EditSelectedCategory()
        {
            int? selectedId = GetSelectedCategoryId();
            if (selectedId == null) return;

            Category cat = Categories.FirstOrDefault(c => c.Id == selectedId.Value);
            if (cat != null)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("اسم التصنيف الجديد:", "تعديل التصنيف", cat.Name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    DatabaseHelper.ExecuteNonQuery($"UPDATE Categories SET Name='{newName}' WHERE Id={cat.Id}");
                    LoadCategories();
                }
            }
        }

        private void DeleteSelectedCategory()
        {
            int? selectedId = GetSelectedCategoryId();
            if (selectedId == null) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذا التصنيف وكل التصنيفات الفرعية؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                DeleteCategoryRecursive(selectedId.Value);
                LoadCategories();
            }
        }

        private void DeleteCategoryRecursive(int categoryId)
        {
            foreach (var child in Categories.Where(c => c.ParentId == categoryId).ToList())
            {
                DeleteCategoryRecursive(child.Id);
            }
            DatabaseHelper.ExecuteNonQuery($"DELETE FROM Categories WHERE Id={categoryId}");
        }

        private void SaveCategories_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم حفظ التصنيفات.");
        }

        private void AddFamily_Click(object sender, RoutedEventArgs e) => AddCategory(null, "Family");
        private void AddSubFamily_Click(object sender, RoutedEventArgs e) => AddCategory(GetSelectedCategoryId(), "SubFamily");
        private void AddSubSubFamily_Click(object sender, RoutedEventArgs e) => AddCategory(GetSelectedCategoryId(), "SubSubFamily");
        private void AddSubSubSubFamily_Click(object sender, RoutedEventArgs e) => AddCategory(GetSelectedCategoryId(), "SubSubSubFamily");
        private void AddSubSubSubSubFamily_Click(object sender, RoutedEventArgs e) => AddCategory(GetSelectedCategoryId(), "SubSubSubSubFamily");

        private void EditFamily_Click(object sender, RoutedEventArgs e) => EditSelectedCategory();
        private void EditSubFamily_Click(object sender, RoutedEventArgs e) => EditSelectedCategory();
        private void EditSubSubFamily_Click(object sender, RoutedEventArgs e) => EditSelectedCategory();
        private void EditSubSubSubFamily_Click(object sender, RoutedEventArgs e) => EditSelectedCategory();
        private void EditSubSubSubSubFamily_Click(object sender, RoutedEventArgs e) => EditSelectedCategory();

        private void DeleteFamily_Click(object sender, RoutedEventArgs e) => DeleteSelectedCategory();
        private void DeleteSubFamily_Click(object sender, RoutedEventArgs e) => DeleteSelectedCategory();
        private void DeleteSubSubFamily_Click(object sender, RoutedEventArgs e) => DeleteSelectedCategory();
        private void DeleteSubSubSubFamily_Click(object sender, RoutedEventArgs e) => DeleteSelectedCategory();
        private void DeleteSubSubSubSubFamily_Click(object sender, RoutedEventArgs e) => DeleteSelectedCategory();

        #endregion

        #region Media, Printing & Export

        private void UploadBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp" };
            if (ofd.ShowDialog() == true)
                DatabaseHelper.SaveBackgroundImage(ofd.FileName);
        }

        private void UploadProductImages_Click(object sender, RoutedEventArgs e) { }
        private void ImportClients_Click(object sender, RoutedEventArgs e) { }
        private void ImportItems_Click(object sender, RoutedEventArgs e) { }
        private void ExportAllData_Click(object sender, RoutedEventArgs e) { }
        private void UploadPrintTemplate_Click(object sender, RoutedEventArgs e) { }
        private void ChooseDefaultPrinter_Click(object sender, RoutedEventArgs e) { }
        private void PreviewPrint_Click(object sender, RoutedEventArgs e) { }

        #endregion

        #region User Permissions

        private void SavePermissions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("تم حفظ الصلاحيات.");
        }

        #endregion

        #region Helpers

        private void LoadAll()
        {
            LoadBranches();
            LoadUsers();
            LoadEmployees();
            LoadAccounts();
            LoadCategories();
        }

        #endregion
    }
}
