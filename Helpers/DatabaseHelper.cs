using ETAG_ERP.Models;
using ETAG_ERP.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using Item = ETAG_ERP.Models.Item;

namespace ETAG_ERP.Helpers
{
    public static class DatabaseHelper
    {
        private static string _dbPath = "ETAG_ERP.db";

        public static void SetDatabasePath(string path)
        {
            _dbPath = path;
            if (!File.Exists(_dbPath))
                SQLiteConnection.CreateFile(_dbPath);
        }

        public static SQLiteConnection GetConnection() => new SQLiteConnection($"Data Source={_dbPath};Version=3;");

        #region Execute Helpers
        public static void ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
        }

        public static DataTable GetDataTable(string sql, params SQLiteParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        #endregion

        #region Reset / Init
        public static void ResetDatabase()
        {
            if (File.Exists(_dbPath)) File.Delete(_dbPath);
            SQLiteConnection.CreateFile(_dbPath);

            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(conn);

            // ================== Clients ==================
            cmd.CommandText = @"
                CREATE TABLE Clients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Phone TEXT, Email TEXT, Address TEXT,
                    Notes TEXT, Balance REAL, TaxCard TEXT, CommercialRecord TEXT,
                    ResponsibleEngineer TEXT, Fax TEXT, BusinessField TEXT,
                    EngineerName TEXT, EvaluationDate TEXT, Evaluator TEXT,
                    CompanyEvaluation TEXT, RatingGood INTEGER, RatingAverage INTEGER, RatingPoor INTEGER
                );"; cmd.ExecuteNonQuery();

            // ================== Users ==================
            cmd.CommandText = @"
                CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT, PasswordHash TEXT, FullName TEXT, IsAdmin INTEGER
                );"; cmd.ExecuteNonQuery();

            // ================== Permissions ==================
            cmd.CommandText = @"
                CREATE TABLE Permissions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Description TEXT
                );"; cmd.ExecuteNonQuery();

            // ================== Branches ==================
            cmd.CommandText = @"
                CREATE TABLE Branches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Address TEXT,
                    Phone TEXT,
                    Type TEXT NOT NULL,       -- Branch/Store/Workshop
                    Location TEXT,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT,
                    CreatedBy TEXT,
                    UpdatedAt TEXT,
                    UpdatedBy TEXT
                );"; cmd.ExecuteNonQuery();

            // ================== Employees ==================
            cmd.CommandText = @"
                CREATE TABLE Employees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT, JobTitle TEXT, Salary REAL, HireDate TEXT,
                    Phone TEXT, Email TEXT, Notes TEXT, Role TEXT
                );"; cmd.ExecuteNonQuery();

            // ================== Accounts ==================
            cmd.CommandText = @"
                CREATE TABLE Accounts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Type TEXT, Balance REAL
                );"; cmd.ExecuteNonQuery();

            // ================== Safes ==================
            cmd.CommandText = @"
                CREATE TABLE Safes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Balance REAL
                );"; cmd.ExecuteNonQuery();

            // ================== Categories ==================
            cmd.CommandText = @"
                CREATE TABLE Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, ParentID INTEGER
                );"; cmd.ExecuteNonQuery();

            // ================== Items ==================
            cmd.CommandText = @"
                CREATE TABLE Items (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ItemName TEXT, Code TEXT, Quantity INTEGER,
                    SellingPrice REAL, PurchasePrice REAL,
                    Price1 REAL, Price2 REAL, Price3 REAL,
                    Cat1 TEXT, Cat2 TEXT, Cat3 TEXT, Cat4 TEXT, Cat5 TEXT,
                    MinStock INTEGER, Description TEXT, Unit TEXT,
                    Barcode TEXT, Tax REAL, Discount REAL, ImagePath TEXT
                );"; cmd.ExecuteNonQuery();

            // ================== Invoices ==================
            cmd.CommandText = @"
                CREATE TABLE Invoices (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceNumber TEXT, ClientId INTEGER, InvoiceDate TEXT,
                    TotalAmount REAL, PaidAmount REAL, Notes TEXT,
                    Status TEXT, Type TEXT, ClientName TEXT
                );"; cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE InvoiceItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId INTEGER, ItemId INTEGER,
                    ItemCode TEXT, ItemName TEXT, Quantity REAL,
                    UnitPrice REAL, Discount REAL
                );"; cmd.ExecuteNonQuery();

            // ================== Returns ==================
            cmd.CommandText = @"
                CREATE TABLE Returns (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReturnNumber TEXT, ClientId INTEGER,
                    InvoiceId INTEGER, ReturnDate TEXT,
                    TotalAmount REAL, Notes TEXT
                );"; cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE ReturnItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReturnId INTEGER, ItemId INTEGER,
                    ItemCode TEXT, ItemName TEXT,
                    Quantity REAL, UnitPrice REAL
                );"; cmd.ExecuteNonQuery();

            // ================== PriceOffers ==================
            cmd.CommandText = @"
                CREATE TABLE PriceOffers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OfferNumber TEXT, ClientId INTEGER,
                    OfferDate TEXT, TotalAmount REAL, Notes TEXT
                );"; cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE PriceOfferItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OfferId INTEGER, ItemId INTEGER,
                    ItemCode TEXT, ItemName TEXT, Quantity REAL, UnitPrice REAL
                );"; cmd.ExecuteNonQuery();

            // ================== Expenses ==================
            cmd.CommandText = @"
                CREATE TABLE Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ExpenseType TEXT, Amount REAL,
                    ExpenseDate TEXT, Description TEXT, Category TEXT
                );"; cmd.ExecuteNonQuery();
        }
        #endregion

        #region Background Image Helper
        public static void SaveBackgroundImage(string imagePath)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string dest = Path.Combine(folder, Path.GetFileName(imagePath));
            File.Copy(imagePath, dest, true);
        }
        #endregion

        #region Clients CRUD
        public static void InsertClient(Client c)
        {
            ExecuteNonQuery(@"INSERT INTO Clients 
                (Name, Phone, Email, Address, Notes, Balance)
                VALUES (@Name,@Phone,@Email,@Address,@Notes,@Balance)",
                new SQLiteParameter("@Name", c.Name),
                new SQLiteParameter("@Phone", c.Phone ?? ""),
                new SQLiteParameter("@Email", c.Email ?? ""),
                new SQLiteParameter("@Address", c.Address ?? ""),
                new SQLiteParameter("@Notes", c.Notes ?? ""),
                new SQLiteParameter("@Balance", c.Balance));
        }

        public static void UpdateClient(Client c)
        {
            ExecuteNonQuery(@"UPDATE Clients SET
                Name=@Name, Phone=@Phone, Email=@Email, Address=@Address, Notes=@Notes, Balance=@Balance
                WHERE Id=@Id",
                new SQLiteParameter("@Id", c.Id),
                new SQLiteParameter("@Name", c.Name),
                new SQLiteParameter("@Phone", c.Phone ?? ""),
                new SQLiteParameter("@Email", c.Email ?? ""),
                new SQLiteParameter("@Address", c.Address ?? ""),
                new SQLiteParameter("@Notes", c.Notes ?? ""),
                new SQLiteParameter("@Balance", c.Balance));
        }

        public static void DeleteClient(int id) => ExecuteNonQuery("DELETE FROM Clients WHERE Id=@Id", new SQLiteParameter("@Id", id));

        public static List<Client> GetClients()
        {
            var dt = GetDataTable("SELECT * FROM Clients");
            var list = new List<Client>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Client
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Phone = row["Phone"].ToString(),
                    Email = row["Email"].ToString(),
                    Address = row["Address"].ToString(),
                    Notes = row["Notes"].ToString(),
                    Balance = Convert.ToDecimal(row["Balance"])
                });
            }
            return list;
        }
        #endregion

        #region Employees CRUD
        public static void InsertEmployee(Employee e)
        {
            ExecuteNonQuery(@"INSERT INTO Employees 
                (FullName, JobTitle, Salary, HireDate, Phone, Email, Notes, Role)
                VALUES (@FullName,@JobTitle,@Salary,@HireDate,@Phone,@Email,@Notes,@Role)",
                new SQLiteParameter("@FullName", e.FullName),
                new SQLiteParameter("@JobTitle", e.JobTitle),
                new SQLiteParameter("@Salary", e.Salary),
                new SQLiteParameter("@HireDate", e.HireDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@Phone", e.Phone ?? ""),
                new SQLiteParameter("@Email", e.Email ?? ""),
                new SQLiteParameter("@Notes", e.Notes ?? ""),
                new SQLiteParameter("@Role", e.Role));
        }

        public static void UpdateEmployee(Employee e)
        {
            ExecuteNonQuery(@"UPDATE Employees SET
                FullName=@FullName, JobTitle=@JobTitle, Salary=@Salary,
                HireDate=@HireDate, Phone=@Phone, Email=@Email, Notes=@Notes, Role=@Role
                WHERE Id=@Id",
                new SQLiteParameter("@Id", e.Id),
                new SQLiteParameter("@FullName", e.FullName),
                new SQLiteParameter("@JobTitle", e.JobTitle),
                new SQLiteParameter("@Salary", e.Salary),
                new SQLiteParameter("@HireDate", e.HireDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@Phone", e.Phone ?? ""),
                new SQLiteParameter("@Email", e.Email ?? ""),
                new SQLiteParameter("@Notes", e.Notes ?? ""),
                new SQLiteParameter("@Role", e.Role));
        }

        public static void DeleteEmployee(int id)
        {
            ExecuteNonQuery("DELETE FROM Employees WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Employee> GetEmployees()
        {
            var dt = GetDataTable("SELECT * FROM Employees");
            var list = new List<Employee>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Employee
                {
                    Id = Convert.ToInt32(row["Id"]),
                    FullName = row["FullName"].ToString(),
                    JobTitle = row["JobTitle"].ToString(),
                    Salary = Convert.ToDecimal(row["Salary"]),
                    HireDate = DateTime.Parse(row["HireDate"].ToString()),
                    Phone = row["Phone"].ToString(),
                    Email = row["Email"].ToString(),
                    Notes = row["Notes"].ToString(),
                    Role = row["Role"].ToString()
                });
            }
            return list;
        }
        #endregion

        #region Permissions CRUD
        public static void InsertPermission(Permission p)
        {
            ExecuteNonQuery(@"INSERT INTO Permissions 
                (Name, Description, UserName, Password, IsAdmin)
                VALUES (@Name,@Description,@UserName,@Password,@IsAdmin)",
                new SQLiteParameter("@Name", p.Name),
                new SQLiteParameter("@Description", p.Description),
                new SQLiteParameter("@UserName", p.UserName),
                new SQLiteParameter("@Password", p.Password),
                new SQLiteParameter("@IsAdmin", p.IsAdmin ? 1 : 0));
        }

        public static void UpdatePermission(Permission p)
        {
            ExecuteNonQuery(@"UPDATE Permissions SET
                Name=@Name, Description=@Description, UserName=@UserName, Password=@Password, IsAdmin=@IsAdmin
                WHERE Id=@Id",
                new SQLiteParameter("@Id", p.Id),
                new SQLiteParameter("@Name", p.Name),
                new SQLiteParameter("@Description", p.Description),
                new SQLiteParameter("@UserName", p.UserName),
                new SQLiteParameter("@Password", p.Password),
                new SQLiteParameter("@IsAdmin", p.IsAdmin ? 1 : 0));
        }

        public static void DeletePermission(int id)
        {
            ExecuteNonQuery("DELETE FROM Permissions WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Permission> GetPermissions()
        {
            var dt = GetDataTable("SELECT * FROM Permissions");
            var list = new List<Permission>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Permission
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString(),
                    UserName = row["UserName"].ToString(),
                    Password = row["Password"].ToString(),
                    IsAdmin = Convert.ToBoolean(row["IsAdmin"])
                });
            }
            return list;
        }
        #endregion

        #region Users CRUD
        public static void InsertUser(User u)
        {
            ExecuteNonQuery(@"INSERT INTO Users
                (Username, PasswordHash, FullName, IsAdmin)
                VALUES (@Username,@PasswordHash,@FullName,@IsAdmin)",
                new SQLiteParameter("@Username", u.Username),
                new SQLiteParameter("@FullName", u.FullName),
                new SQLiteParameter("@IsAdmin", u.IsAdmin ? 1 : 0));
        }

        public static void UpdateUser(User u)
        {
            ExecuteNonQuery(@"UPDATE Users SET
                Username=@Username, PasswordHash=@PasswordHash, FullName=@FullName, IsAdmin=@IsAdmin
                WHERE Id=@Id",
                new SQLiteParameter("@Id", u.Id),
                new SQLiteParameter("@Username", u.Username),
                new SQLiteParameter("@FullName", u.FullName),
                new SQLiteParameter("@IsAdmin", u.IsAdmin ? 1 : 0));
        }

        public static void DeleteUser(int id)
        {
            ExecuteNonQuery("DELETE FROM Users WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<User> GetUsers()
        {
            var dt = GetDataTable("SELECT * FROM Users");
            var list = new List<User>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new User
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString(),
                    FullName = row["FullName"].ToString(),
                    IsAdmin = Convert.ToBoolean(row["IsAdmin"])
                });
            }
            return list;
        }
        #endregion

        #region Accounts CRUD
        public static void InsertAccount(Models.Account a)
        {
            ExecuteNonQuery(@"INSERT INTO Accounts
                (Name, Type, Balance)
                VALUES (@Name,@Type,@Balance)",
                new SQLiteParameter("@Name", a.Name),
                new SQLiteParameter("@Type", a.Type),
                new SQLiteParameter("@Balance", a.Balance));
        }

        public static void UpdateAccount(Models.Account a)
        {
            ExecuteNonQuery(@"UPDATE Accounts SET
                Name=@Name, Type=@Type, Balance=@Balance
                WHERE Id=@Id",
                new SQLiteParameter("@Id", a.Id),
                new SQLiteParameter("@Name", a.Name),
                new SQLiteParameter("@Type", a.Type),
                new SQLiteParameter("@Balance", a.Balance));
        }

        public static void DeleteAccount(int id)
        {
            ExecuteNonQuery("DELETE FROM Accounts WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Models.Account> GetAccounts()
        {
            var dt = GetDataTable("SELECT * FROM Accounts");
            var list = new List<Models.Account>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Models.Account
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Type = row["Type"].ToString(),
                    Balance = Convert.ToDecimal(row["Balance"])
                });
            }
            return list;
        }
        #endregion

        #region Safes CRUD
        public static void InsertSafe(Safe s)
        {
            ExecuteNonQuery(@"INSERT INTO Safes
                (Name, Balance)
                VALUES (@Name,@Balance)",
                new SQLiteParameter("@Name", s.Name),
                new SQLiteParameter("@Balance", s.Balance));
        }

        public static void UpdateSafe(Safe s)
        {
            ExecuteNonQuery(@"UPDATE Safes SET
                Name=@Name, Balance=@Balance
                WHERE Id=@Id",
                new SQLiteParameter("@Id", s.Id),
                new SQLiteParameter("@Name", s.Name),
                new SQLiteParameter("@Balance", s.Balance));
        }

        public static void DeleteSafe(int id)
        {
            ExecuteNonQuery("DELETE FROM Safes WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Safe> GetSafes()
        {
            var dt = GetDataTable("SELECT * FROM Safes");
            var list = new List<Safe>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Safe
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Balance = Convert.ToDecimal(row["Balance"])
                });
            }
            return list;
        }
        #endregion

        #region Branches CRUD
        public static void InsertBranch(Models.Branch b)
        {
            ExecuteNonQuery(@"INSERT INTO Branches
                (Name, Address, Phone, IsActive)
                VALUES (@Name,@Address,@Phone,@IsActive)",
                new SQLiteParameter("@Name", b.Name),
                new SQLiteParameter("@Address", b.Address),
                new SQLiteParameter("@Phone", b.Phone),
                new SQLiteParameter("@IsActive", b.IsActive ? 1 : 0));
        }

        public static void UpdateBranch(Models.Branch b)
        {
            ExecuteNonQuery(@"UPDATE Branches SET
                Name=@Name, Address=@Address, Phone=@Phone, IsActive=@IsActive
                WHERE Id=@Id",
                new SQLiteParameter("@Id", b.Id),
                new SQLiteParameter("@Name", b.Name),
                new SQLiteParameter("@Address", b.Address),
                new SQLiteParameter("@Phone", b.Phone),
                new SQLiteParameter("@IsActive", b.IsActive ? 1 : 0));
        }

        public static void DeleteBranch(int id)
        {
            ExecuteNonQuery("DELETE FROM Branches WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Models.Branch> GetBranches()
        {
            var dt = GetDataTable("SELECT * FROM Branches");
            var list = new List<Models.Branch>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Models.Branch
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Address = row["Address"].ToString(),
                    Phone = row["Phone"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }
            return list;
        }
        #endregion

        #region Categories CRUD
        public static void InsertCategory(Category c)
        {
            ExecuteNonQuery(@"INSERT INTO Categories
                (Name, ParentID)
                VALUES (@Name,@ParentID)",
                new SQLiteParameter("@Name", c.Name),
                new SQLiteParameter("@ParentID", c.ParentID));
        }

        public static void UpdateCategory(Category c)
        {
            ExecuteNonQuery(@"UPDATE Categories SET
                Name=@Name, ParentID=@ParentID
                WHERE Id=@Id",
                new SQLiteParameter("@Id", c.Id),
                new SQLiteParameter("@Name", c.Name),
                new SQLiteParameter("@ParentID", c.ParentID));
        }

        public static void DeleteCategory(int id)
        {
            ExecuteNonQuery("DELETE FROM Categories WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Category> GetCategories()
        {
            var dt = GetDataTable("SELECT * FROM Categories");
            var list = new List<Category>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    ParentID = Convert.ToInt32(row["ParentID"])
                });
            }
            return list;
        }
        #endregion

        #region Items CRUD
        public static void InsertItem(Item i)
        {
            ExecuteNonQuery(@"INSERT INTO Items
                (ItemName, Code, Quantity, SellingPrice, PurchasePrice, Price1, Price2, Price3,
                 Cat1, Cat2, Cat3, Cat4, Cat5, MinStock, Description, Unit, Barcode, Tax, Discount, ImagePath)
                VALUES
                (@ItemName,@Code,@Quantity,@SellingPrice,@PurchasePrice,@Price1,@Price2,@Price3,
                 @Cat1,@Cat2,@Cat3,@Cat4,@Cat5,@MinStock,@Description,@Unit,@Barcode,@Tax,@Discount,@ImagePath)",
                new SQLiteParameter("@ItemName", i.ItemName),
                new SQLiteParameter("@Code", i.Code),
                new SQLiteParameter("@Quantity", i.Quantity),
                new SQLiteParameter("@SellingPrice", i.SellingPrice),
                new SQLiteParameter("@PurchasePrice", i.PurchasePrice),
                new SQLiteParameter("@Price1", i.Price1),
                new SQLiteParameter("@Price2", i.Price2),
                new SQLiteParameter("@Price3", i.Price3),
                new SQLiteParameter("@Cat1", i.Cat1),
                new SQLiteParameter("@Cat2", i.Cat2),
                new SQLiteParameter("@Cat3", i.Cat3),
                new SQLiteParameter("@Cat4", i.Cat4),
                new SQLiteParameter("@Cat5", i.Cat5),
                new SQLiteParameter("@MinStock", i.MinStock),
                new SQLiteParameter("@Description", i.Description),
                new SQLiteParameter("@Unit", i.Unit),
                new SQLiteParameter("@Barcode", i.Barcode),
                new SQLiteParameter("@Tax", i.Tax),
                new SQLiteParameter("@Discount", i.Discount),
                new SQLiteParameter("@ImagePath", i.ImagePath));
        }

        public static void UpdateItem(Item i)
        {
            ExecuteNonQuery(@"UPDATE Items SET
                ItemName=@ItemName, Code=@Code, Quantity=@Quantity, SellingPrice=@SellingPrice,
                PurchasePrice=@PurchasePrice, Price1=@Price1, Price2=@Price2, Price3=@Price3,
                Cat1=@Cat1, Cat2=@Cat2, Cat3=@Cat3, Cat4=@Cat4, Cat5=@Cat5,
                MinStock=@MinStock, Description=@Description, Unit=@Unit, Barcode=@Barcode, Tax=@Tax,
                Discount=@Discount, ImagePath=@ImagePath
                WHERE Id=@Id",
                new SQLiteParameter("@Id", i.Id),
                new SQLiteParameter("@ItemName", i.ItemName),
                new SQLiteParameter("@Code", i.Code),
                new SQLiteParameter("@Quantity", i.Quantity),
                new SQLiteParameter("@SellingPrice", i.SellingPrice),
                new SQLiteParameter("@PurchasePrice", i.PurchasePrice),
                new SQLiteParameter("@Price1", i.Price1),
                new SQLiteParameter("@Price2", i.Price2),
                new SQLiteParameter("@Price3", i.Price3),
                new SQLiteParameter("@Cat1", i.Cat1),
                new SQLiteParameter("@Cat2", i.Cat2),
                new SQLiteParameter("@Cat3", i.Cat3),
                new SQLiteParameter("@Cat4", i.Cat4),
                new SQLiteParameter("@Cat5", i.Cat5),
                new SQLiteParameter("@MinStock", i.MinStock),
                new SQLiteParameter("@Description", i.Description),
                new SQLiteParameter("@Unit", i.Unit),
                new SQLiteParameter("@Barcode", i.Barcode),
                new SQLiteParameter("@Tax", i.Tax),
                new SQLiteParameter("@Discount", i.Discount),
                new SQLiteParameter("@ImagePath", i.ImagePath));
        }

        public static void DeleteItem(int id)
        {
            ExecuteNonQuery("DELETE FROM Items WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Item> GetItems()
        {
            var dt = GetDataTable("SELECT * FROM Items");
            var list = new List<Item>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Item
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ItemName = row["ItemName"].ToString(),
                    Code = row["Code"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    SellingPrice = Convert.ToDecimal(row["SellingPrice"]),
                    PurchasePrice = Convert.ToDecimal(row["PurchasePrice"]),
                    Price1 = Convert.ToDecimal(row["Price1"]),
                    Price2 = Convert.ToDecimal(row["Price2"]),
                    Price3 = Convert.ToDecimal(row["Price3"]),
                    Cat1 = row["Cat1"].ToString(),
                    Cat2 = row["Cat2"].ToString(),
                    Cat3 = row["Cat3"].ToString(),
                    Cat4 = row["Cat4"].ToString(),
                    Cat5 = row["Cat5"].ToString(),
                    MinStock = Convert.ToInt32(row["MinStock"]),
                    Description = row["Description"].ToString(),
                    Unit = row["Unit"].ToString(),
                    Barcode = row["Barcode"].ToString(),
                    Tax = Convert.ToDecimal(row["Tax"]),
                    Discount = Convert.ToDecimal(row["Discount"]),
                    ImagePath = row["ImagePath"].ToString()
                });
            }
            return list;
        }
        #endregion

        #region Invoices CRUD
        public static void InsertInvoice(Invoice i)
        {
            ExecuteNonQuery(@"INSERT INTO Invoices
                (InvoiceNumber, ClientId, InvoiceDate, TotalAmount, PaidAmount, Notes, Status, Type, ClientName)
                VALUES
                (@InvoiceNumber, @ClientId, @InvoiceDate, @TotalAmount, @PaidAmount, @Notes, @Status, @Type, @ClientName)",
                new SQLiteParameter("@InvoiceNumber", i.InvoiceNumber),
                new SQLiteParameter("@ClientId", i.ClientId),
                new SQLiteParameter("@TotalAmount", i.TotalAmount),
                new SQLiteParameter("@PaidAmount", i.PaidAmount),
                new SQLiteParameter("@Notes", i.Notes ?? ""),
                new SQLiteParameter("@Status", i.Status),
                new SQLiteParameter("@Type", i.Type),
                new SQLiteParameter("@ClientName", i.ClientName));
        }

        public static void UpdateInvoice(Invoice i)
        {
            ExecuteNonQuery(@"UPDATE Invoices SET
                InvoiceNumber=@InvoiceNumber, ClientId=@ClientId, InvoiceDate=@InvoiceDate, TotalAmount=@TotalAmount,
                PaidAmount=@PaidAmount, Notes=@Notes, Status=@Status, Type=@Type, ClientName=@ClientName
                WHERE Id=@Id",
                new SQLiteParameter("@Id", i.Id),
                new SQLiteParameter("@InvoiceNumber", i.InvoiceNumber),
                new SQLiteParameter("@ClientId", i.ClientId),
                new SQLiteParameter("@TotalAmount", i.TotalAmount),
                new SQLiteParameter("@PaidAmount", i.PaidAmount),
                new SQLiteParameter("@Notes", i.Notes ?? ""),
                new SQLiteParameter("@Status", i.Status),
                new SQLiteParameter("@Type", i.Type),
                new SQLiteParameter("@ClientName", i.ClientName));
        }

        public static void DeleteInvoice(int id)
        {
            ExecuteNonQuery("DELETE FROM Invoices WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Invoice> GetInvoices()
        {
            var dt = GetDataTable("SELECT * FROM Invoices");
            var list = new List<Invoice>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Invoice
                {
                    Id = Convert.ToInt32(row["Id"]),
                    InvoiceNumber = row["InvoiceNumber"].ToString(),
                    ClientId = Convert.ToInt32(row["ClientId"]),
                    InvoiceDate = DateTime.Parse(row["InvoiceDate"].ToString()),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    PaidAmount = Convert.ToDecimal(row["PaidAmount"]),
                    Notes = row["Notes"].ToString(),
                    Status = row["Status"].ToString(),
                    Type = row["Type"].ToString(),
                    ClientName = row["ClientName"].ToString()
                });
            }
            return list;
        }
        #endregion

        #region InvoiceItems CRUD
        public static void InsertInvoiceItem(InvoiceItem i)
        {
            ExecuteNonQuery(@"INSERT INTO InvoiceItems
                (InvoiceId, ItemId, ItemCode, ItemName, Quantity, UnitPrice, Discount)
                VALUES (@InvoiceId, @ItemId, @ItemCode, @ItemName, @Quantity, @UnitPrice, @Discount)",
                new SQLiteParameter("@InvoiceId", i.InvoiceId),
                new SQLiteParameter("@ItemId", i.ItemId),
                new SQLiteParameter("@ItemCode", i.ItemCode),
                new SQLiteParameter("@ItemName", i.ItemName),
                new SQLiteParameter("@Quantity", i.Quantity),
                new SQLiteParameter("@UnitPrice", i.UnitPrice),
                new SQLiteParameter("@Discount", i.Discount));
        }

        public static void UpdateInvoiceItem(InvoiceItem i)
        {
            ExecuteNonQuery(@"UPDATE InvoiceItems SET
                InvoiceId=@InvoiceId, ItemId=@ItemId, ItemCode=@ItemCode, ItemName=@ItemName,
                Quantity=@Quantity, UnitPrice=@UnitPrice, Discount=@Discount
                WHERE Id=@Id",
                new SQLiteParameter("@Id", i.Id),
                new SQLiteParameter("@InvoiceId", i.InvoiceId),
                new SQLiteParameter("@ItemId", i.ItemId),
                new SQLiteParameter("@ItemCode", i.ItemCode),
                new SQLiteParameter("@ItemName", i.ItemName),
                new SQLiteParameter("@Quantity", i.Quantity),
                new SQLiteParameter("@UnitPrice", i.UnitPrice),
                new SQLiteParameter("@Discount", i.Discount));
        }

        public static void DeleteInvoiceItem(int id)
        {
            ExecuteNonQuery("DELETE FROM InvoiceItems WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<InvoiceItem> GetInvoiceItems(int invoiceId)
        {
            var dt = GetDataTable("SELECT * FROM InvoiceItems WHERE InvoiceId=@InvoiceId", new SQLiteParameter("@InvoiceId", invoiceId));
            var list = new List<InvoiceItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new InvoiceItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    InvoiceId = Convert.ToInt32(row["InvoiceId"]),
                    ItemId = Convert.ToInt32(row["ItemId"]),
                    ItemCode = row["ItemCode"].ToString(),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = Convert.ToDecimal(row["Quantity"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    Discount = Convert.ToDecimal(row["Discount"])
                });
            }
            return list;
        }
        #endregion

        #region Returns CRUD
        public static void InsertReturn(Return r)
        {
            ExecuteNonQuery(@"INSERT INTO Returns
                (ReturnNumber, ClientId, InvoiceId, ReturnDate, TotalAmount, Notes)
                VALUES
                (@ReturnNumber, @ClientId, @InvoiceId, @ReturnDate, @TotalAmount, @Notes)",
                new SQLiteParameter("@ReturnNumber", r.ReturnNumber),
                new SQLiteParameter("@ClientId", r.ClientId),
                new SQLiteParameter("@InvoiceId", r.InvoiceId),
                new SQLiteParameter("@ReturnDate", r.ReturnDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@TotalAmount", r.TotalAmount),
                new SQLiteParameter("@Notes", r.Notes ?? ""));
        }

        public static void UpdateReturn(Return r)
        {
            ExecuteNonQuery(@"UPDATE Returns SET
                ReturnNumber=@ReturnNumber, ClientId=@ClientId, InvoiceId=@InvoiceId,
                ReturnDate=@ReturnDate, TotalAmount=@TotalAmount, Notes=@Notes
                WHERE Id=@Id",
                new SQLiteParameter("@Id", r.Id),
                new SQLiteParameter("@ReturnNumber", r.ReturnNumber),
                new SQLiteParameter("@ClientId", r.ClientId),
                new SQLiteParameter("@InvoiceId", r.InvoiceId),
                new SQLiteParameter("@ReturnDate", r.ReturnDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@TotalAmount", r.TotalAmount),
                new SQLiteParameter("@Notes", r.Notes ?? ""));
        }

        public static void DeleteReturn(int id)
        {
            ExecuteNonQuery("DELETE FROM Returns WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Return> GetReturns()
        {
            var dt = GetDataTable("SELECT * FROM Returns");
            var list = new List<Return>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Return
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ReturnNumber = row["ReturnNumber"].ToString(),
                    ClientId = Convert.ToInt32(row["ClientId"]),
                    InvoiceId = Convert.ToInt32(row["InvoiceId"]),
                    ReturnDate = DateTime.Parse(row["ReturnDate"].ToString()),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Notes = row["Notes"].ToString()
                });
            }
            return list;
        }
        #endregion

        #region ReturnItems CRUD
        public static void InsertReturnItem(ReturnItem r)
        {
            ExecuteNonQuery(@"INSERT INTO ReturnItems
                (ReturnId, ItemId, ItemCode, ItemName, Quantity, UnitPrice)
                VALUES (@ReturnId, @ItemId, @ItemCode, @ItemName, @Quantity, @UnitPrice)",
                new SQLiteParameter("@ReturnId", r.ReturnId),
                new SQLiteParameter("@ItemId", r.ItemId),
                new SQLiteParameter("@ItemCode", r.ItemCode),
                new SQLiteParameter("@ItemName", r.ItemName),
                new SQLiteParameter("@Quantity", r.Quantity),
                new SQLiteParameter("@UnitPrice", r.UnitPrice));
        }

        public static void UpdateReturnItem(ReturnItem r)
        {
            ExecuteNonQuery(@"UPDATE ReturnItems SET
                ReturnId=@ReturnId, ItemId=@ItemId, ItemCode=@ItemCode, ItemName=@ItemName,
                Quantity=@Quantity, UnitPrice=@UnitPrice
                WHERE Id=@Id",
                new SQLiteParameter("@Id", r.Id),
                new SQLiteParameter("@ReturnId", r.ReturnId),
                new SQLiteParameter("@ItemId", r.ItemId),
                new SQLiteParameter("@ItemCode", r.ItemCode),
                new SQLiteParameter("@ItemName", r.ItemName),
                new SQLiteParameter("@Quantity", r.Quantity),
                new SQLiteParameter("@UnitPrice", r.UnitPrice));
        }

        public static void DeleteReturnItem(int id)
        {
            ExecuteNonQuery("DELETE FROM ReturnItems WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<ReturnItem> GetReturnItems(int returnId)
        {
            var dt = GetDataTable("SELECT * FROM ReturnItems WHERE ReturnId=@ReturnId", new SQLiteParameter("@ReturnId", returnId));
            var list = new List<ReturnItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ReturnItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ReturnId = Convert.ToInt32(row["ReturnId"]),
                    ItemId = Convert.ToInt32(row["ItemId"]),
                    ItemCode = row["ItemCode"].ToString(),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = Convert.ToDecimal(row["Quantity"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"])
                });
            }
            return list;
        }
        #endregion

        #region PriceOffers CRUD
        public static void InsertPriceOffer(PriceOffer p)
        {
            ExecuteNonQuery(@"INSERT INTO PriceOffers
                (OfferNumber, ClientId, OfferDate, TotalAmount, Notes)
                VALUES (@OfferNumber, @ClientId, @OfferDate, @TotalAmount, @Notes)",
                new SQLiteParameter("@OfferNumber", p.OfferNumber),
                new SQLiteParameter("@ClientId", p.ClientId),
                new SQLiteParameter("@OfferDate", p.OfferDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@TotalAmount", p.TotalAmount),
                new SQLiteParameter("@Notes", p.Notes ?? ""));
        }

        public static void UpdatePriceOffer(PriceOffer p)
        {
            ExecuteNonQuery(@"UPDATE PriceOffers SET
                OfferNumber=@OfferNumber, ClientId=@ClientId, OfferDate=@OfferDate, TotalAmount=@TotalAmount, Notes=@Notes
                WHERE Id=@Id",
                new SQLiteParameter("@Id", p.Id),
                new SQLiteParameter("@OfferNumber", p.OfferNumber),
                new SQLiteParameter("@ClientId", p.ClientId),
                new SQLiteParameter("@OfferDate", p.OfferDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@TotalAmount", p.TotalAmount),
                new SQLiteParameter("@Notes", p.Notes ?? ""));
        }

        public static void DeletePriceOffer(int id)
        {
            ExecuteNonQuery("DELETE FROM PriceOffers WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<PriceOffer> GetPriceOffers()
        {
            var dt = GetDataTable("SELECT * FROM PriceOffers");
            var list = new List<PriceOffer>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PriceOffer
                {
                    Id = Convert.ToInt32(row["Id"]),
                    OfferNumber = row["OfferNumber"].ToString(),
                    ClientId = Convert.ToInt32(row["ClientId"]),
                    OfferDate = DateTime.Parse(row["OfferDate"].ToString()),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Notes = row["Notes"].ToString()
                });
            }
            return list;
        }
        #endregion

        #region PriceOfferItems CRUD
        public static void InsertPriceOfferItem(PriceOfferItem p)
        {
            ExecuteNonQuery(@"INSERT INTO PriceOfferItems
                (OfferId, ItemId, ItemCode, ItemName, Quantity, UnitPrice)
                VALUES (@OfferId, @ItemId, @ItemCode, @ItemName, @Quantity, @UnitPrice)",
                new SQLiteParameter("@OfferId", p.OfferId),
                new SQLiteParameter("@ItemId", p.ItemId),
                new SQLiteParameter("@ItemCode", p.ItemCode),
                new SQLiteParameter("@ItemName", p.ItemName),
                new SQLiteParameter("@Quantity", p.Quantity),
                new SQLiteParameter("@UnitPrice", p.UnitPrice));
        }

        public static void UpdatePriceOfferItem(PriceOfferItem p)
        {
            ExecuteNonQuery(@"UPDATE PriceOfferItems SET
                OfferId=@OfferId, ItemId=@ItemId, ItemCode=@ItemCode, ItemName=@ItemName,
                Quantity=@Quantity, UnitPrice=@UnitPrice
                WHERE Id=@Id",
                new SQLiteParameter("@Id", p.Id),
                new SQLiteParameter("@OfferId", p.OfferId),
                new SQLiteParameter("@ItemId", p.ItemId),
                new SQLiteParameter("@ItemCode", p.ItemCode),
                new SQLiteParameter("@ItemName", p.ItemName),
                new SQLiteParameter("@Quantity", p.Quantity),
                new SQLiteParameter("@UnitPrice", p.UnitPrice));
        }

        public static void DeletePriceOfferItem(int id)
        {
            ExecuteNonQuery("DELETE FROM PriceOfferItems WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<PriceOfferItem> GetPriceOfferItems(int offerId)
        {
            var dt = GetDataTable("SELECT * FROM PriceOfferItems WHERE OfferId=@OfferId", new SQLiteParameter("@OfferId", offerId));
            var list = new List<PriceOfferItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PriceOfferItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    OfferId = Convert.ToInt32(row["OfferId"]),
                    ItemId = Convert.ToInt32(row["ItemId"]),
                    ItemCode = row["ItemCode"].ToString(),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = Convert.ToDecimal(row["Quantity"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"])
                });
            }
            return list;
        }
        #endregion

        #region Expenses CRUD
        public static void InsertExpense(Expense e)
        {
            ExecuteNonQuery(@"INSERT INTO Expenses
                (ExpenseType, Amount, ExpenseDate, Description, Category)
                VALUES (@ExpenseType, @Amount, @ExpenseDate, @Description, @Category)",
                new SQLiteParameter("@ExpenseType", e.ExpenseType),
                new SQLiteParameter("@Amount", e.Amount),
                new SQLiteParameter("@Description", e.Description),
                new SQLiteParameter("@Category", e.Category));
        }

        public static void UpdateExpense(Expense e)
        {
            ExecuteNonQuery(@"UPDATE Expenses SET
                ExpenseType=@ExpenseType, Amount=@Amount, ExpenseDate=@ExpenseDate, Description=@Description, Category=@Category
                WHERE Id=@Id",
                new SQLiteParameter("@Id", e.Id),
                new SQLiteParameter("@ExpenseType", e.ExpenseType),
                new SQLiteParameter("@Amount", e.Amount),
                new SQLiteParameter("@Description", e.Description),
                new SQLiteParameter("@Category", e.Category));
        }

        public static void DeleteExpense(int id)
        {
            ExecuteNonQuery("DELETE FROM Expenses WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public static List<Expense> GetExpenses()
        {
            var dt = GetDataTable("SELECT * FROM Expenses");
            var list = new List<Expense>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Expense
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ExpenseType = row["ExpenseType"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    ExpenseDate = DateTime.Parse(row["ExpenseDate"].ToString()),
                    Description = row["Description"].ToString(),
                    Category = row["Category"].ToString()
                });
            }
            return list;
        }
        #endregion

        #region Transactions
        public static void AddTransaction(TransactionModel tx)
        {
            ExecuteNonQuery(@"
                INSERT INTO Transactions (Date, Type, Reference, Amount, Username, Description, AccountId, ClientId, InvoiceNumber)
                VALUES (@Date, @Type, @Reference, @Amount, @Username, @Description, @AccountId, @ClientId, @InvoiceNumber)",
                new SQLiteParameter("@Date", tx.Date.ToString("s")),
                new SQLiteParameter("@Type", tx.Type),
                new SQLiteParameter("@Reference", tx.Reference ?? ""),
                new SQLiteParameter("@Amount", tx.Amount),
                new SQLiteParameter("@Username", tx.Username ?? ""),
                new SQLiteParameter("@Description", tx.Description ?? ""),
                new SQLiteParameter("@AccountId", tx.AccountId ?? (object)DBNull.Value),
                new SQLiteParameter("@ClientId", tx.ClientId ?? (object)DBNull.Value),
                new SQLiteParameter("@InvoiceNumber", tx.InvoiceNumber ?? ""));
        }

        internal static void DeleteClient(Client selected)
        {
            throw new NotImplementedException();
        }

        internal static List<Client>? GetAllClients()
        {
            throw new NotImplementedException();
        }

        internal static List<Invoice> GetAllInvoices()
        {
            throw new NotImplementedException();
        }

        internal static List<Item>? GetAllItems()
        {
            throw new NotImplementedException();
        }

        internal static Invoice? GetInvoiceById(int invoiceId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}