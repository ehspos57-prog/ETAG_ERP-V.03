using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;

public static class DatabaseHelper
{
    private static string _dbPath = "ETAG_ERP.db";
    private static readonly string _connectionString = $"Data Source={_dbPath};Version=3;";


    public static string ConnectionString { get; internal set; }

    public static void SetDatabasePath(string path)
    {
        _dbPath = path;
        if (!File.Exists(_dbPath))
            SQLiteConnection.CreateFile(_dbPath);
    }

    public static SQLiteConnection GetConnection() =>
        new SQLiteConnection($"Data Source={_dbPath};Version=3;");

    /// <summary>
    /// إنشاء قاعدة البيانات والجداول والتأكد من الأعمدة
    /// </summary>
    public static void InitializeDatabase()
    {
        if (!File.Exists(_dbPath))
            SQLiteConnection.CreateFile(_dbPath);

        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SQLiteCommand(conn);

        // ================== الجداول الأساسية ==================
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Settings (
                Key TEXT PRIMARY KEY,
                Value TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT,
                PasswordHash TEXT,
                FullName TEXT,
                Role TEXT,
                IsAdmin INTEGER
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Permissions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Description TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS UserPermissions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER,
                PermissionId INTEGER,
                FOREIGN KEY(UserId) REFERENCES Users(Id),
                FOREIGN KEY(PermissionId) REFERENCES Permissions(Id)
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Branches (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Address TEXT,
                Phone TEXT,
                Type TEXT,
                IsActive INTEGER,
                CreatedAt TEXT,
                CreatedBy TEXT,
                UpdatedAt TEXT,
                UpdatedBy TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Employees (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT,
                JobTitle TEXT,
                Salary REAL,
                HireDate TEXT,
                Phone TEXT,
                Email TEXT,
                Notes TEXT,
                Role TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Accounts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Type TEXT,
                Balance REAL,
                OpeningBalance REAL,
                Description TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Safes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Balance REAL
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                ParentId INTEGER NULL,
                Type TEXT,
                Description TEXT,
                IsActive INTEGER,
                CreatedAt TEXT,
                UpdatedAt TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ItemName TEXT,
                Code TEXT,
                Quantity INTEGER,
                SellingPrice REAL,
                PurchasePrice REAL,
                Price1 REAL,
                Price2 REAL,
                Price3 REAL,
                Cat1 TEXT,
                Cat2 TEXT,
                Cat3 TEXT,
                Cat4 TEXT,
                Cat5 TEXT,
                MinStock INTEGER,
                Description TEXT,
                Unit TEXT,
                Barcode TEXT,
                Tax REAL,
                Discount REAL,
                ImagePath TEXT,
                CreatedAt TEXT,
                UpdatedAt TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Clients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Phone TEXT,
                Email TEXT,
                Address TEXT,
                Notes TEXT,
                Balance REAL,
                TaxCard TEXT,
                CommercialRecord TEXT,
                ResponsibleEngineer TEXT,
                Fax TEXT,
                BusinessField TEXT,
                EngineerName TEXT,
                EvaluationDate TEXT,
                Evaluator TEXT,
                CompanyEvaluation TEXT,
                RatingGood INTEGER,
                RatingAverage INTEGER,
                RatingPoor INTEGER
            );";
        cmd.ExecuteNonQuery();

        // Invoices and lines
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Invoices (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceNumber TEXT,
                ClientId INTEGER,
                InvoiceDate TEXT,
                TotalAmount REAL,
                PaidAmount REAL,
                Notes TEXT,
                Status TEXT,
                Type TEXT,
                ClientName TEXT,
                CreatedBy TEXT,
                UpdatedAt TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS InvoiceItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceId INTEGER,
                ItemId INTEGER,
                ItemCode TEXT,
                ItemName TEXT,
                Quantity REAL,
                UnitPrice REAL,
                Discount REAL,
                FOREIGN KEY(InvoiceId) REFERENCES Invoices(Id),
                FOREIGN KEY(ItemId) REFERENCES Items(Id)
            );";
        cmd.ExecuteNonQuery();

        // Returns
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Returns (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ReturnNumber TEXT,
                ClientId INTEGER,
                InvoiceId INTEGER,
                ReturnDate TEXT,
                TotalAmount REAL,
                Notes TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS ReturnItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ReturnId INTEGER,
                ItemId INTEGER,
                ItemCode TEXT,
                ItemName TEXT,
                Quantity REAL,
                UnitPrice REAL,
                FOREIGN KEY(ReturnId) REFERENCES Returns(Id),
                FOREIGN KEY(ItemId) REFERENCES Items(Id)
            );";
        cmd.ExecuteNonQuery();

        // Purchases & lines
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Purchases (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PurchaseNumber TEXT,
                SupplierId INTEGER,
                PurchaseDate TEXT,
                TotalAmount REAL,
                PaidAmount REAL,
                Notes TEXT,
                CreatedBy TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS PurchaseItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PurchaseId INTEGER,
                ItemId INTEGER,
                ItemCode TEXT,
                ItemName TEXT,
                Quantity REAL,
                UnitPrice REAL,
                FOREIGN KEY(PurchaseId) REFERENCES Purchases(Id),
                FOREIGN KEY(ItemId) REFERENCES Items(Id)
            );";
        cmd.ExecuteNonQuery();

        // Price offers (quotes)
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS PriceOffers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OfferNumber TEXT,
                ClientId INTEGER,
                OfferDate TEXT,
                TotalAmount REAL,
                Notes TEXT
            );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS PriceOfferItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OfferId INTEGER,
                ItemId INTEGER,
                ItemCode TEXT,
                ItemName TEXT,
                Quantity REAL,
                UnitPrice REAL
            );";
        cmd.ExecuteNonQuery();

        // Expenses
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Expenses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ExpenseType TEXT,
                Amount REAL,
                ExpenseDate TEXT,
                Description TEXT,
                Category TEXT
            );";
        cmd.ExecuteNonQuery();

        // Print templates storage (Stimulsoft / FastReport templates saved as files path or blob)
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS PrintTemplates (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TemplateName TEXT,
                TemplatePath TEXT,
                TemplateType TEXT, -- e.g. Stimulsoft, FastReport
                CreatedAt TEXT
            );";
        cmd.ExecuteNonQuery();

        // Ensure any missing important columns on existing tables (backward-compatible)
        EnsureColumns(conn, "Branches", new Dictionary<string, string>
        {
            {"Type","TEXT"},
            {"IsActive","INTEGER"},
            {"CreatedAt","TEXT"},
            {"CreatedBy","TEXT"},
            {"UpdatedAt","TEXT"},
            {"UpdatedBy","TEXT"}
        });

        EnsureColumns(conn, "Categories", new Dictionary<string, string>
        {
            {"Type","TEXT"},
            {"Description","TEXT"},
            {"IsActive","INTEGER"},
            {"CreatedAt","TEXT"},
            {"UpdatedAt","TEXT"}
        });

        EnsureColumns(conn, "Invoices", new Dictionary<string, string>
        {
            {"CreatedBy","TEXT"},
            {"UpdatedAt","TEXT"}
        });

        EnsureColumns(conn, "Items", new Dictionary<string, string>
        {
            {"CreatedAt","TEXT"},
            {"UpdatedAt","TEXT"}
        });

        // إغلاق الاتصال
        conn.Close();
    }

    /// <summary>
    /// يتأكد من وجود الأعمدة ويضيف أي ناقص
    /// </summary>
    private static void EnsureColumns(SQLiteConnection conn, string tableName, Dictionary<string, string> columns)
    {
        using var cmd = new SQLiteCommand($"PRAGMA table_info([{tableName}]);", conn);
        using var reader = cmd.ExecuteReader();

        var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (reader.Read())
        {
            existing.Add(reader["name"].ToString());
        }

        foreach (var col in columns)
        {
            if (!existing.Contains(col.Key))
            {
                using var alter = new SQLiteCommand($"ALTER TABLE [{tableName}] ADD COLUMN [{col.Key}] {col.Value};", conn);
                alter.ExecuteNonQuery();
            }
        }
    }

    // ================== Execute Helpers ==================
    public static int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SQLiteCommand(sql, conn);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();  // ترجع عدد الصفوف المتأثرة
    }

    public static DataTable GetCategories(int? parentId = null)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            string query;

            if (parentId == null)
                query = "SELECT Id, Name FROM Categories WHERE ParentId IS NULL";
            else
                query = "SELECT Id, Name FROM Categories WHERE ParentId = @ParentId";

            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parentId != null)
                    cmd.Parameters.AddWithValue("@ParentId", parentId);

                using (var da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // ✅ هنا نمنع إضافة الأعمدة لو كانت موجودة
                    if (!dt.Columns.Contains("Id"))
                        dt.Columns.Add("Id", typeof(int));
                    if (!dt.Columns.Contains("Name"))
                        dt.Columns.Add("Name", typeof(string));

                    // ✅ لو الجدول فاضي (مفيش داتا في DB) نحمل من الـ Seeder
                    if (dt.Rows.Count == 0 && parentId == null)
                    {
                        int id = 1;
                        foreach (var name in CategorySeeder.GetSeedData()
                            .Select(c => c.Level1)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct())
                        {
                            var row = dt.NewRow();
                            row["Id"] = id++;
                            row["Name"] = name;
                            dt.Rows.Add(row);
                        }
                    }

                    return dt;
                }
            }
        }
    }


    public static DataTable GetDataTable(string sql, params SQLiteParameter[] parameters)
    {
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            using var cmd = new SQLiteCommand(sql, conn);
            if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            using var adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database Error: {ex.Message}");
            throw;
        }
        return dt;
    }

    public static object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SQLiteCommand(sql, conn);
        if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    // ================== Settings helpers ==================
    public static void SetSetting(string key, string value)
    {
        ExecuteNonQuery("INSERT OR REPLACE INTO Settings (Key, Value) VALUES (@k, @v);",
            new SQLiteParameter("@k", key), new SQLiteParameter("@v", value));
    }

    public static string GetSetting(string key)
    {
        var dt = GetDataTable("SELECT Value FROM Settings WHERE Key=@k;", new SQLiteParameter("@k", key));
        if (dt.Rows.Count == 0) return null;
        return dt.Rows[0]["Value"]?.ToString();
    }

    public static void SaveBackgroundImage(string imagePath)
    {
        ExecuteNonQuery("CREATE TABLE IF NOT EXISTS Settings (Key TEXT PRIMARY KEY, Value TEXT);");
        SetSetting("BackgroundImage", imagePath);
    }

    // ================== Print templates ==================
    public static int SavePrintTemplate(string templateName, string templatePath, string templateType)
    {
        string sql = @"INSERT INTO PrintTemplates (TemplateName, TemplatePath, TemplateType, CreatedAt)
                       VALUES (@name, @path, @type, @created);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@name", templateName),
            new SQLiteParameter("@path", templatePath),
            new SQLiteParameter("@type", templateType),
            new SQLiteParameter("@created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static DataTable GetPrintTemplates()
    {
        return GetDataTable("SELECT * FROM PrintTemplates ORDER BY Id DESC;");
    }

    public static void DeletePrintTemplate(int id)
    {
        ExecuteNonQuery("DELETE FROM PrintTemplates WHERE Id=@id;", new SQLiteParameter("@id", id));
    }

    // ================== Users / Permissions ==================
    public static int InsertUser(User user)
    {
        string sql = @"INSERT INTO Users (Username, PasswordHash, FullName, Role, IsAdmin)
                       VALUES (@Username, @PasswordHash, @FullName, @Role, @IsAdmin);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Username", user.Username ?? ""),
            new SQLiteParameter("@PasswordHash", user.Password ?? ""),
            new SQLiteParameter("@FullName", user.FullName ?? ""),
            new SQLiteParameter("@Role", user.Role ?? ""),
            new SQLiteParameter("@IsAdmin", user.IsAdmin ? 1 : 0));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateUser(User user)
    {
        string sql = @"UPDATE Users SET Username=@Username, PasswordHash=@PasswordHash, FullName=@FullName, Role=@Role, IsAdmin=@IsAdmin WHERE Id=@Id;";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Username", user.Username ?? ""),
            new SQLiteParameter("@PasswordHash", user.Password ?? ""),
            new SQLiteParameter("@FullName", user.FullName ?? ""),
            new SQLiteParameter("@Role", user.Role ?? ""),
            new SQLiteParameter("@IsAdmin", user.IsAdmin ? 1 : 0),
            new SQLiteParameter("@Id", user.Id));
    }

    public static void DeleteUser(int userId)
    {
        ExecuteNonQuery("DELETE FROM UserPermissions WHERE UserId=@id;", new SQLiteParameter("@id", userId));
        ExecuteNonQuery("DELETE FROM Users WHERE Id=@id;", new SQLiteParameter("@id", userId));
    }

    public static List<User> GetAllUsers()
    {
        var dt = GetDataTable("SELECT * FROM Users;");
        var list = new List<User>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Username = row["Username"]?.ToString(),
                FullName = row["FullName"]?.ToString(),
                Role = row.Table.Columns.Contains("Role") ? row["Role"]?.ToString() : "",
                IsAdmin = row.Table.Columns.Contains("IsAdmin") && row["IsAdmin"] != DBNull.Value ? Convert.ToBoolean(row["IsAdmin"]) : false
            });
        }
        return list;
    }

    public static int InsertPermission(string name, string description = "")
    {
        ExecuteNonQuery("INSERT INTO Permissions (Name, Description) VALUES (@n, @d);",
            new SQLiteParameter("@n", name), new SQLiteParameter("@d", description));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static List<(int Id, string Name)> GetAllPermissions()
    {
        var dt = GetDataTable("SELECT Id, Name FROM Permissions;");
        var list = new List<(int, string)>();
        foreach (DataRow r in dt.Rows) list.Add((Convert.ToInt32(r["Id"]), r["Name"]?.ToString()));
        return list;
    }

    public static void AssignPermissionToUser(int userId, int permissionId)
    {
        ExecuteNonQuery("INSERT INTO UserPermissions (UserId, PermissionId) VALUES (@u, @p);",
            new SQLiteParameter("@u", userId), new SQLiteParameter("@p", permissionId));
    }

    public static void RemovePermissionFromUser(int userId, int permissionId)
    {
        ExecuteNonQuery("DELETE FROM UserPermissions WHERE UserId=@u AND PermissionId=@p;",
            new SQLiteParameter("@u", userId), new SQLiteParameter("@p", permissionId));
    }

    public static List<int> GetUserPermissions(int userId)
    {
        var dt = GetDataTable("SELECT PermissionId FROM UserPermissions WHERE UserId=@u;", new SQLiteParameter("@u", userId));
        var list = new List<int>();
        foreach (DataRow r in dt.Rows) list.Add(Convert.ToInt32(r["PermissionId"]));
        return list;
    }

    // ================== Branches ==================
    public static int InsertBranch(Branch b)
    {
        string sql = @"INSERT INTO Branches (Name, Address, Phone, Type, IsActive, CreatedAt, CreatedBy) 
                       VALUES (@Name,@Address,@Phone,@Type,@IsActive,@CreatedAt,@CreatedBy);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Name", b.Name ?? ""),
            new SQLiteParameter("@Address", b.Address ?? ""),
            new SQLiteParameter("@Phone", b.Phone ?? ""),
            new SQLiteParameter("@Type", b.Type ?? ""),
            new SQLiteParameter("@IsActive", b.IsActive ? 1 : 0),
            new SQLiteParameter("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new SQLiteParameter("@CreatedBy", b.CreatedBy ?? ""));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateBranch(Branch b)
    {
        ExecuteNonQuery(@"UPDATE Branches SET Name=@Name, Address=@Address, Phone=@Phone, Type=@Type, IsActive=@IsActive, UpdatedAt=@UpdatedAt, UpdatedBy=@UpdatedBy WHERE Id=@Id;",
            new SQLiteParameter("@Name", b.Name ?? ""),
            new SQLiteParameter("@Address", b.Address ?? ""),
            new SQLiteParameter("@Phone", b.Phone ?? ""),
            new SQLiteParameter("@Type", b.Type ?? ""),
            new SQLiteParameter("@IsActive", b.IsActive ? 1 : 0),
            new SQLiteParameter("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new SQLiteParameter("@UpdatedBy", b.UpdatedBy ?? ""),
            new SQLiteParameter("@Id", b.Id));
    }

    public static void DeleteBranch(int id) => ExecuteNonQuery("DELETE FROM Branches WHERE Id=@Id;", new SQLiteParameter("@Id", id));

    public static List<Branch> GetAllBranches()
    {
        var dt = GetDataTable("SELECT * FROM Branches;");
        var list = new List<Branch>();
        foreach (DataRow r in dt.Rows)
        {
            list.Add(new Branch
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = r["Name"]?.ToString(),
                Address = r["Address"]?.ToString(),
                Phone = r["Phone"]?.ToString(),
                Type = r.Table.Columns.Contains("Type") ? r["Type"]?.ToString() : null,
                IsActive = r.Table.Columns.Contains("IsActive") && r["IsActive"] != DBNull.Value ? Convert.ToBoolean(r["IsActive"]) : true
            });
        }
        return list;
    }

    // ================== Employees ==================
    public static int InsertEmployee(Employee emp)
    {
        string sql = @"INSERT INTO Employees (FullName, JobTitle, Salary, HireDate, Phone, Email, Notes, Role)
                       VALUES (@FullName,@JobTitle,@Salary,@HireDate,@Phone,@Email,@Notes,@Role);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@FullName", emp.FullName),
            new SQLiteParameter("@JobTitle", emp.JobTitle),
            new SQLiteParameter("@Salary", emp.Salary),
            new SQLiteParameter("@HireDate", emp.HireDate),
            new SQLiteParameter("@Phone", emp.Phone ?? ""),
            new SQLiteParameter("@Email", emp.Email ?? ""),
            new SQLiteParameter("@Notes", emp.Notes ?? ""),
            new SQLiteParameter("@Role", emp.Role ?? ""));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateEmployee(Employee emp)
    {
        ExecuteNonQuery(@"UPDATE Employees SET FullName=@FullName, JobTitle=@JobTitle, Salary=@Salary, HireDate=@HireDate, Phone=@Phone, Email=@Email, Notes=@Notes, Role=@Role WHERE Id=@Id;",
            new SQLiteParameter("@FullName", emp.FullName ?? ""),
            new SQLiteParameter("@JobTitle", emp.JobTitle ?? ""),
            new SQLiteParameter("@Salary", emp.Salary),
            new SQLiteParameter("@HireDate", emp.HireDate),
            new SQLiteParameter("@Phone", emp.Phone ?? ""),
            new SQLiteParameter("@Email", emp.Email ?? ""),
            new SQLiteParameter("@Notes", emp.Notes ?? ""),
            new SQLiteParameter("@Role", emp.Role ?? ""),
            new SQLiteParameter("@Id", emp.Id));
    }

    public static void DeleteEmployee(int id) => ExecuteNonQuery("DELETE FROM Employees WHERE Id=@Id;", new SQLiteParameter("@Id", id));

    public static List<Employee> GetAllEmployees()
    {
        var dt = GetDataTable("SELECT * FROM Employees;");
        var list = new List<Employee>();
        foreach (DataRow r in dt.Rows)
        {
            list.Add(new Employee
            {
                Id = Convert.ToInt32(r["Id"]),
                FullName = r["FullName"]?.ToString(),
                JobTitle = r["JobTitle"]?.ToString(),
                Salary = r.Table.Columns.Contains("Salary") && r["Salary"] != DBNull.Value ? Convert.ToDecimal(r["Salary"]) : 0,

                Phone = r["Phone"]?.ToString(),
                Email = r["Email"]?.ToString()
            });
        }
        return list;
    }

    // ================== Accounts / Safes ==================
    public static int InsertAccount(Account acc)
    {
        SQLiteParameter sQLiteParameter = new("@b", acc.Balance);
        ExecuteNonQuery(@"INSERT INTO Accounts (Name, Type, Balance, OpeningBalance, Description) VALUES (@n,@t,@b,@ob,@d);",
            new SQLiteParameter("@n", acc.Name ?? ""),
            new SQLiteParameter("@t", acc.Type ?? ""),
            new SQLiteParameter("@b", acc.Balance),
            new SQLiteParameter("@ob", acc.OpeningBalance),
            new SQLiteParameter("@d", acc.Description));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateAccount(Account acc)
    {
        ExecuteNonQuery(@"UPDATE Accounts SET Name=@n, Type=@t, Balance=@b, OpeningBalance=@ob, Description=@d WHERE Id=@Id;",
            new SQLiteParameter("@n", acc.Name ?? ""),
            new SQLiteParameter("@t", acc.Type ?? ""),
            new SQLiteParameter("@b", acc.Balance),
            new SQLiteParameter("@Id", acc.Id));
    }

    public static void DeleteAccount(int id) => ExecuteNonQuery("DELETE FROM Accounts WHERE Id=@Id;", new SQLiteParameter("@Id", id));

    public static List<Account> GetAllAccounts()
    {
        var dt = GetDataTable("SELECT * FROM Accounts;");
        var list = new List<Account>();
        foreach (DataRow r in dt.Rows)
        {
            list.Add(new Account
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = r["Name"]?.ToString(),
                Type = r["Type"]?.ToString(),
                Balance = r.Table.Columns.Contains("Balance") && r["Balance"] != DBNull.Value ? Convert.ToDecimal(r["Balance"]) : 0
            });
        }
        return list;
    }

    // ================== Categories ==================
    public static int InsertCategory(Category c)
    {
        ExecuteNonQuery("INSERT INTO Categories (Name, ParentId, Type, Description, IsActive, CreatedAt) VALUES (@n,@p,@t,@d,@ia,@ca);",
            new SQLiteParameter("@n", c.Name ?? ""),
            new SQLiteParameter("@p", c.ParentId == 0 ? (object)DBNull.Value : c.ParentId),
            new SQLiteParameter("@t", c.Type ?? ""),
            new SQLiteParameter("@d", c.Description),
            new SQLiteParameter("@ia", c.IsActive ? 1 : 0),
            new SQLiteParameter("@ca", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateCategory(Category c)
    {
        ExecuteNonQuery("UPDATE Categories SET Name=@n, ParentId=@p, Type=@t, Description=@d, IsActive=@ia, UpdatedAt=@ua WHERE Id=@Id;",
            new SQLiteParameter("@n", c.Name ?? ""),
            new SQLiteParameter("@p", c.ParentId == 0 ? (object)DBNull.Value : c.ParentId),
            new SQLiteParameter("@t", c.Type ?? ""),
            new SQLiteParameter("@d", c.Description),
            new SQLiteParameter("@ia", c.IsActive ? 1 : 0),
            new SQLiteParameter("@ua", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new SQLiteParameter("@Id", c.Id));
    }

    public static void DeleteCategory(int id) => ExecuteNonQuery("DELETE FROM Categories WHERE Id=@Id;", new SQLiteParameter("@Id", id));

    public static List<Category> GetAllCategories()
    {
        var categories = new List<Category>();

        try
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            using var cmd = new SQLiteCommand("SELECT * FROM Categories ORDER BY Id;", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(new Category
                {
                    Level1 = reader["Level1"]?.ToString(),
                    Level2 = reader["Level2"]?.ToString(),
                    Level3 = reader["Level3"]?.ToString(),
                    Level4 = reader["Level4"]?.ToString(),
                    Level5 = reader["Level5"]?.ToString(),
                    Code = reader["Code"]?.ToString()
                });
            }

            // ✅ في حالة الجدول فاضي (أول تشغيل)
            if (categories.Count == 0)
            {
                categories = GetCategorySeedData(); // دالة بنضفها زي ما وضحتلك قبل كده
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء تحميل التصنيفات: {ex.Message}");
        }

        return categories;
    }

    private static List<Category> GetCategorySeedData()
    {
        var seed = ETAG_ERP.Helpers.CategorySeeder.GetSeedData() ?? new List<CategorySeedData>();

        var list = seed.Select(s => new Category
        {
            Level1 = s.Level1,
            Level2 = s.Level2,
            Level3 = s.Level3,
            Level4 = s.Level4,
            Level5 = s.Level5,
            Code = s.Code ?? ""
        }).ToList();

        return list;
    }

    public static void InsertCategoriesIfEmpty(List<CategorySeedData> seedData)
    {
        try
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            using var cmdCheck = new SQLiteCommand("SELECT COUNT(*) FROM Categories;", conn);
            long count = (long)cmdCheck.ExecuteScalar();
            if (count > 0) return; // لو فيها بيانات بالفعل، ما نضيفش السيدنج تاني

            foreach (var c in seedData)
            {
                using var cmd = new SQLiteCommand(@"
                INSERT INTO Categories (Level1, Level2, Level3, Level4, Level5, Code)
                VALUES (@l1,@l2,@l3,@l4,@l5,@code);", conn);

                cmd.Parameters.AddWithValue("@l1", c.Level1 ?? "");
                cmd.Parameters.AddWithValue("@l2", c.Level2 ?? "");
                cmd.Parameters.AddWithValue("@l3", c.Level3 ?? "");
                cmd.Parameters.AddWithValue("@l4", c.Level4 ?? "");
                cmd.Parameters.AddWithValue("@l5", c.Level5 ?? "");
                cmd.Parameters.AddWithValue("@code", c.Code ?? "");
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء حفظ التصنيفات الابتدائية: {ex.Message}");
        }
    }
    // ================== Items ==================
    public static bool InsertItem(Item item)
    {
        try
        {
            string query = @"
            INSERT INTO Items 
                (ItemName, Code, Quantity, SellingPrice, PurchasePrice, Price1, Price2, Price3,
                 Cat1, Cat2, Cat3, Cat4, Cat5, MinStock, Description, Unit, Barcode, Tax, Discount, ImagePath, CreatedAt, UpdatedAt)
            VALUES
                (@ItemName, @Code, @Quantity, @SellingPrice, @PurchasePrice, @Price1, @Price2, @Price3,
                 @Cat1, @Cat2, @Cat3, @Cat4, @Cat5, @MinStock, @Description, @Unit, @Barcode, @Tax, @Discount, @ImagePath, @CreatedAt, @UpdatedAt)
        ";

            int affected = ExecuteNonQuery(query,
                new SQLiteParameter("@ItemName", item.ItemName ?? ""),
                new SQLiteParameter("@Code", item.Code ?? ""),
                new SQLiteParameter("@Quantity", item.Quantity),
                new SQLiteParameter("@SellingPrice", item.SellingPrice),
                new SQLiteParameter("@PurchasePrice", item.PurchasePrice),
                new SQLiteParameter("@Price1", item.Price1),
                new SQLiteParameter("@Price2", item.Price2),
                new SQLiteParameter("@Price3", item.Price3),
                new SQLiteParameter("@Cat1", item.Cat1 ?? ""),
                new SQLiteParameter("@Cat2", item.Cat2 ?? ""),
                new SQLiteParameter("@Cat3", item.Cat3 ?? ""),
                new SQLiteParameter("@Cat4", item.Cat4 ?? ""),
                new SQLiteParameter("@Cat5", item.Cat5 ?? ""),
                new SQLiteParameter("@MinStock", item.MinStock),
                new SQLiteParameter("@Description", item.Description ?? ""),
                new SQLiteParameter("@Unit", item.Unit ?? ""),
                new SQLiteParameter("@Barcode", item.Barcode ?? ""),
                new SQLiteParameter("@Tax", item.Tax),
                new SQLiteParameter("@Discount", item.Discount),
                new SQLiteParameter("@ImagePath", item.ImagePath ?? ""),
                new SQLiteParameter("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new SQLiteParameter("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            );

            return affected > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء إضافة الصنف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public static bool UpdateItem(Item item)
    {
        try
        {
            string query = @"
        UPDATE Items SET 
            ItemName=@ItemName,
            Code=@Code,
            Quantity=@Quantity,
            SellingPrice=@SellingPrice,
            PurchasePrice=@PurchasePrice,
            Price1=@Price1,
            Price2=@Price2,
            Price3=@Price3,
            Cat1=@Cat1,
            Cat2=@Cat2,
            Cat3=@Cat3,
            Cat4=@Cat4,
            Cat5=@Cat5,
            MinStock=@MinStock,
            Description=@Description,
            Unit=@Unit,
            Barcode=@Barcode,
            Tax=@Tax,
            Discount=@Discount,
            ImagePath=@ImagePath,
            UpdatedAt=@UpdatedAt
        WHERE Id=@Id
    ";

            int affected = ExecuteNonQuery(query,
                new SQLiteParameter("@ItemName", item.ItemName ?? ""),
                new SQLiteParameter("@Code", item.Code ?? ""),
                new SQLiteParameter("@Quantity", item.Quantity),
                new SQLiteParameter("@SellingPrice", item.SellingPrice),
                new SQLiteParameter("@PurchasePrice", item.PurchasePrice),
                new SQLiteParameter("@Price1", item.Price1),
                new SQLiteParameter("@Price2", item.Price2),
                new SQLiteParameter("@Price3", item.Price3),
                new SQLiteParameter("@Cat1", item.Cat1 ?? ""),
                new SQLiteParameter("@Cat2", item.Cat2 ?? ""),
                new SQLiteParameter("@Cat3", item.Cat3 ?? ""),
                new SQLiteParameter("@Cat4", item.Cat4 ?? ""),
                new SQLiteParameter("@Cat5", item.Cat5 ?? ""),
                new SQLiteParameter("@MinStock", item.MinStock),
                new SQLiteParameter("@Description", item.Description ?? ""),
                new SQLiteParameter("@Unit", item.Unit ?? ""),
                new SQLiteParameter("@Barcode", item.Barcode ?? ""),
                new SQLiteParameter("@Tax", item.Tax),
                new SQLiteParameter("@Discount", item.Discount),
                new SQLiteParameter("@ImagePath", item.ImagePath ?? ""),
                new SQLiteParameter("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new SQLiteParameter("@Id", item.ItemID)
            );

            return affected > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء تحديث الصنف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }



    public static bool DeleteItem(int id)
    {
        int result = ExecuteNonQuery("DELETE FROM Items WHERE Id=@Id", new SQLiteParameter("@Id", id));
        return result > 0;
    }

    public static List<Item> GetAllItems()
    {
        var dt = GetDataTable("SELECT * FROM Items;");
        var list = new List<Item>();
        foreach (DataRow row in dt.Rows)
            list.Add(MapItem(row));
        return list;

    }

    private static Item MapItem(DataRow row) => new Item
    {
        Id = Convert.ToInt32(row["Id"]),
        ItemName = row.Table.Columns.Contains("ItemName") ? row["ItemName"]?.ToString() : "",
        Code = row.Table.Columns.Contains("Code") ? row["Code"]?.ToString() : "",
        Quantity = row.Table.Columns.Contains("Quantity") && row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0,
        SellingPrice = row.Table.Columns.Contains("SellingPrice") && row["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(row["SellingPrice"]) : 0,
        PurchasePrice = row.Table.Columns.Contains("PurchasePrice") && row["PurchasePrice"] != DBNull.Value ? Convert.ToDecimal(row["PurchasePrice"]) : 0,
        Price1 = row.Table.Columns.Contains("Price1") && row["Price1"] != DBNull.Value ? Convert.ToDecimal(row["Price1"]) : 0,
        Price2 = row.Table.Columns.Contains("Price2") && row["Price2"] != DBNull.Value ? Convert.ToDecimal(row["Price2"]) : 0,
        Price3 = row.Table.Columns.Contains("Price3") && row["Price3"] != DBNull.Value ? Convert.ToDecimal(row["Price3"]) : 0,
        Cat1 = row.Table.Columns.Contains("Cat1") ? row["Cat1"]?.ToString() : null,
        Cat2 = row.Table.Columns.Contains("Cat2") ? row["Cat2"]?.ToString() : null,
        Cat3 = row.Table.Columns.Contains("Cat3") ? row["Cat3"]?.ToString() : null,
        Cat4 = row.Table.Columns.Contains("Cat4") ? row["Cat4"]?.ToString() : null,
        Cat5 = row.Table.Columns.Contains("Cat5") ? row["Cat5"]?.ToString() : null,
        MinStock = row.Table.Columns.Contains("MinStock") && row["MinStock"] != DBNull.Value ? Convert.ToInt32(row["MinStock"]) : 0,
        Description = row.Table.Columns.Contains("Description") ? row["Description"]?.ToString() : null,
        Unit = row.Table.Columns.Contains("Unit") ? row["Unit"]?.ToString() : null,
        Barcode = row.Table.Columns.Contains("Barcode") ? row["Barcode"]?.ToString() : null,
        Tax = row.Table.Columns.Contains("Tax") && row["Tax"] != DBNull.Value ? Convert.ToDecimal(row["Tax"]) : 0,
        Discount = row.Table.Columns.Contains("Discount") && row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0,
        ImagePath = row.Table.Columns.Contains("ImagePath") ? row["ImagePath"]?.ToString() : null
    };

    // ================== Clients ==================
    public static int InsertClient(Client client)
    {
        string sql = @"INSERT INTO Clients (Name, Phone, Email, Address, Notes, Balance, TaxCard, CommercialRecord, ResponsibleEngineer, Fax, BusinessField, EngineerName, EvaluationDate, Evaluator, CompanyEvaluation, RatingGood, RatingAverage, RatingPoor)
                   VALUES (@Name,@Phone,@Email,@Address,@Notes,@Balance,@TaxCard,@CommercialRecord,@ResponsibleEngineer,@Fax,@BusinessField,@EngineerName,@EvaluationDate,@Evaluator,@CompanyEvaluation,@RatingGood,@RatingAverage,@RatingPoor);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Name", client.Name ?? ""),
            new SQLiteParameter("@Phone", client.Phone ?? ""),
            new SQLiteParameter("@Email", client.Email ?? ""),
            new SQLiteParameter("@Address", client.Address ?? ""),
            new SQLiteParameter("@Notes", client.Notes ?? ""),
            new SQLiteParameter("@Balance", client.Balance),
            new SQLiteParameter("@TaxCard", client.TaxCard ?? ""),
            new SQLiteParameter("@CommercialRecord", client.CommercialRecord ?? ""),
            new SQLiteParameter("@ResponsibleEngineer", client.ResponsibleEngineer ?? ""),
            new SQLiteParameter("@Fax", client.Fax ?? ""),
            new SQLiteParameter("@BusinessField", client.BusinessField ?? ""),
            new SQLiteParameter("@EngineerName", client.EngineerName ?? ""),
            new SQLiteParameter("@EvaluationDate", client.EvaluationDate?.ToString() ?? ""),
            new SQLiteParameter("@Evaluator", client.Evaluator ?? ""),
            new SQLiteParameter("@CompanyEvaluation", client.CompanyEvaluation ?? ""),
            new SQLiteParameter("@RatingGood", client.RatingGood ? 1 : 0),
            new SQLiteParameter("@RatingAverage", client.RatingAverage ? 1 : 0),
            new SQLiteParameter("@RatingPoor", client.RatingPoor ? 1 : 0));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateClient(Client client)
    {
        string sql = @"UPDATE Clients SET Name=@Name, Phone=@Phone, Email=@Email, Address=@Address,
                   Notes=@Notes, Balance=@Balance, TaxCard=@TaxCard, CommercialRecord=@CommercialRecord,
                   ResponsibleEngineer=@ResponsibleEngineer, Fax=@Fax, BusinessField=@BusinessField,
                   EngineerName=@EngineerName, EvaluationDate=@EvaluationDate, Evaluator=@Evaluator,
                   CompanyEvaluation=@CompanyEvaluation, RatingGood=@RatingGood, RatingAverage=@RatingAverage, RatingPoor=@RatingPoor
                   WHERE Id=@Id;";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Name", client.Name ?? ""),
            new SQLiteParameter("@Phone", client.Phone ?? ""),
            new SQLiteParameter("@Email", client.Email ?? ""),
            new SQLiteParameter("@Address", client.Address ?? ""),
            new SQLiteParameter("@Notes", client.Notes ?? ""),
            new SQLiteParameter("@Balance", client.Balance),
            new SQLiteParameter("@TaxCard", client.TaxCard ?? ""),
            new SQLiteParameter("@CommercialRecord", client.CommercialRecord ?? ""),
            new SQLiteParameter("@ResponsibleEngineer", client.ResponsibleEngineer ?? ""),
            new SQLiteParameter("@Fax", client.Fax ?? ""),
            new SQLiteParameter("@BusinessField", client.BusinessField ?? ""),
            new SQLiteParameter("@EngineerName", client.EngineerName ?? ""),
            new SQLiteParameter("@EvaluationDate", client.EvaluationDate?.ToString() ?? ""),
            new SQLiteParameter("@Evaluator", client.Evaluator ?? ""),
            new SQLiteParameter("@CompanyEvaluation", client.CompanyEvaluation ?? ""),
            new SQLiteParameter("@RatingGood", client.RatingGood ? 1 : 0),
            new SQLiteParameter("@RatingAverage", client.RatingAverage ? 1 : 0),
            new SQLiteParameter("@RatingPoor", client.RatingPoor ? 1 : 0),
            new SQLiteParameter("@Id", client.Id));
    }

    public static void DeleteClient(int clientId)
    {
        ExecuteNonQuery("DELETE FROM Clients WHERE Id=@Id;", new SQLiteParameter("@Id", clientId));
    }

    private static Client MapClient(DataRow row) => new Client
    {
        Id = Convert.ToInt32(row["Id"]),
        Name = row["Name"]?.ToString() ?? "",
        Phone = row["Phone"]?.ToString(),
        Email = row["Email"]?.ToString(),
        Address = row["Address"]?.ToString(),
        Notes = row["Notes"]?.ToString(),
        Balance = row.Table.Columns.Contains("Balance") && row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0,
        TaxCard = row.Table.Columns.Contains("TaxCard") ? row["TaxCard"]?.ToString() : null,
        CommercialRecord = row.Table.Columns.Contains("CommercialRecord") ? row["CommercialRecord"]?.ToString() : null,
        Fax = row.Table.Columns.Contains("Fax") ? row["Fax"]?.ToString() : null,
        BusinessField = row.Table.Columns.Contains("BusinessField") ? row["BusinessField"]?.ToString() : null
    };

    public static List<Client> GetAllClients()
    {
        var dt = GetDataTable("SELECT * FROM Clients;");
        var list = new List<Client>();
        foreach (DataRow row in dt.Rows)
            list.Add(MapClient(row));
        return list;
    }

    // ================== Expenses ==================
    public static int InsertExpense(Expense expense)
    {
        string sql = @"INSERT INTO Expenses (ExpenseType, Amount, ExpenseDate, Description, Category)
                   VALUES (@ExpenseType, @Amount, @ExpenseDate, @Description, @Category);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@ExpenseType", expense.ExpenseType ?? ""),
            new SQLiteParameter("@Amount", expense.Amount),
            new SQLiteParameter("@ExpenseDate", expense.ExpenseDate ?? null),
            new SQLiteParameter("@Description", expense.Description ?? ""),
            new SQLiteParameter("@Category", expense.Category ?? ""));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateExpense(Expense expense)
    {
        string sql = @"UPDATE Expenses SET ExpenseType=@ExpenseType, Amount=@Amount, 
                   ExpenseDate=@ExpenseDate, Description=@Description, Category=@Category 
                   WHERE Id=@Id;";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@ExpenseType", expense.ExpenseType ?? ""),
            new SQLiteParameter("@Amount", expense.Amount),
            new SQLiteParameter("@ExpenseDate", expense.ExpenseDate ?? null),
            new SQLiteParameter("@Description", expense.Description ?? ""),
            new SQLiteParameter("@Category", expense.Category ?? ""),
            new SQLiteParameter("@Id", expense.Id));
    }

    public static void DeleteExpense(int id) => ExecuteNonQuery("DELETE FROM Expenses WHERE Id=@Id;", new SQLiteParameter("@Id", id));

    public static List<Expense> GetAllExpenses()
    {
        var dt = GetDataTable("SELECT * FROM Expenses;");
        var list = new List<Expense>();
        foreach (DataRow r in dt.Rows)
        {
            list.Add(new Expense
            {
                Id = Convert.ToInt32(r["Id"]),
                ExpenseType = r["ExpenseType"]?.ToString(),
                Amount = r.Table.Columns.Contains("Amount") && r["Amount"] != DBNull.Value ? Convert.ToDecimal(r["Amount"]) : 0,
                Description = r["Description"]?.ToString(),
                Category = r["Category"]?.ToString()
            });
        }
        return list;
    }

    // ================== Purchases (مشتريات) ==================
    public static int InsertPurchase(Purchase purchase)
    {
        string sql = @"INSERT INTO Purchases (PurchaseNumber, SupplierId, PurchaseDate, TotalAmount, PaidAmount, Notes, CreatedBy)
                       VALUES (@num,@sup,@date,@total,@paid,@notes,@createdBy);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@num", purchase.PurchaseNumber ?? ""),
            new SQLiteParameter("@sup", purchase.SupplierId),
            new SQLiteParameter("@date", purchase.PurchaseDate),
            new SQLiteParameter("@total", purchase.TotalAmount),
            new SQLiteParameter("@paid", purchase.PaidAmount),
            new SQLiteParameter("@notes", purchase.Notes ?? ""),
            new SQLiteParameter("@createdBy", purchase.CreatedBy ?? ""));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdatePurchase(Purchase purchase)
    {
        ExecuteNonQuery(@"UPDATE Purchases SET PurchaseNumber=@num, SupplierId=@sup, PurchaseDate=@date, TotalAmount=@total, PaidAmount=@paid, Notes=@notes WHERE Id=@Id;",
            new SQLiteParameter("@num", purchase.PurchaseNumber ?? ""),
            new SQLiteParameter("@sup", purchase.SupplierId),
            new SQLiteParameter("@date", purchase.PurchaseDate),
            new SQLiteParameter("@total", purchase.TotalAmount),
            new SQLiteParameter("@paid", purchase.PaidAmount),
            new SQLiteParameter("@notes", purchase.Notes ?? ""),
            new SQLiteParameter("@Id", purchase.Id));
    }

    public static void DeletePurchase(int id) => ExecuteNonQuery("DELETE FROM Purchases WHERE Id=@Id;", new SQLiteParameter("@Id", id));


    public static List<Purchase> GetAllPurchases()
    {
        var dt = GetDataTable("SELECT * FROM Purchases;");
        var list = new List<Purchase>();
        foreach (DataRow r in dt.Rows)
        {
            list.Add(new Purchase
            {
                Id = Convert.ToInt32(r["Id"]),
                PurchaseNumber = r["PurchaseNumber"]?.ToString(),
                SupplierId = r.Table.Columns.Contains("SupplierId") && r["SupplierId"] != DBNull.Value ? Convert.ToInt32(r["SupplierId"]) : 0,
                TotalAmount = r.Table.Columns.Contains("TotalAmount") && r["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(r["TotalAmount"]) : 0,
                PaidAmount = r.Table.Columns.Contains("PaidAmount") && r["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(r["PaidAmount"]) : 0,
                Notes = r["Notes"]?.ToString()
            });
        }
        return list;
    }

    // ================== PurchaseItems ==================
    public static int InsertPurchaseItem(PurchaseItem pi)
    {
        ExecuteNonQuery(@"INSERT INTO PurchaseItems (PurchaseId, ItemId, ItemCode, ItemName, Quantity, UnitPrice)
                         VALUES (@pid,@iid,@code,@name,@qty,@price);",
            new SQLiteParameter("@pid", pi.PurchaseId),
            new SQLiteParameter("@iid", pi.ItemId),
            new SQLiteParameter("@code", pi.ItemCode ?? ""),
            new SQLiteParameter("@name", pi.ItemName ?? ""),
            new SQLiteParameter("@qty", pi.Quantity),
            new SQLiteParameter("@price", pi.UnitPrice));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void DeletePurchaseItem(int id) => ExecuteNonQuery("DELETE FROM PurchaseItems WHERE Id=@Id;", new SQLiteParameter("@Id", id));

    // ================== Invoices ==================
    public static int InsertInvoice(Invoice inv)
    {
        string sql = @"INSERT INTO Invoices (InvoiceNumber, ClientId, InvoiceDate, TotalAmount, PaidAmount, Notes, Status, Type, ClientName, CreatedBy)
                       VALUES (@num,@client,@date,@total,@paid,@notes,@status,@type,@cname,@createdBy);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@num", inv.InvoiceNumber ?? ""),
            new SQLiteParameter("@client", inv.ClientId),
            new SQLiteParameter("@date", inv.InvoiceDate ?? ""),
            new SQLiteParameter("@total", inv.TotalAmount),
            new SQLiteParameter("@paid", inv.PaidAmount),
            new SQLiteParameter("@notes", inv.Notes ?? ""),
            new SQLiteParameter("@status", inv.Status ?? ""),
            new SQLiteParameter("@type", inv.Type ?? ""),
            new SQLiteParameter("@cname", inv.ClientName ?? ""),
            new SQLiteParameter("@createdBy", inv.CreatedBy ?? ""));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateInvoice(Invoice inv)
    {
        ExecuteNonQuery(@"UPDATE Invoices SET InvoiceNumber=@num, ClientId=@client, InvoiceDate=@date, TotalAmount=@total, PaidAmount=@paid, Notes=@notes, Status=@status, Type=@type, ClientName=@cname WHERE Id=@Id;",
            new SQLiteParameter("@num", inv.InvoiceNumber ?? ""),
            new SQLiteParameter("@client", inv.ClientId),
            new SQLiteParameter("@date", inv.InvoiceDate ?? ""),
            new SQLiteParameter("@total", inv.TotalAmount),
            new SQLiteParameter("@paid", inv.PaidAmount),
            new SQLiteParameter("@notes", inv.Notes ?? ""),
            new SQLiteParameter("@status", inv.Status ?? ""),
            new SQLiteParameter("@type", inv.Type ?? ""),
            new SQLiteParameter("@cname", inv.ClientName ?? ""),
            new SQLiteParameter("@Id", inv.Id));
    }

    public static void DeleteInvoice(int id)
    {
        ExecuteNonQuery("DELETE FROM InvoiceItems WHERE InvoiceId=@Id;", new SQLiteParameter("@Id", id));
        ExecuteNonQuery("DELETE FROM Invoices WHERE Id=@Id;", new SQLiteParameter("@Id", id));
    }

    public static List<Invoice> GetAllInvoices()
    {
        var dt = GetDataTable("SELECT * FROM Invoices;");
        var list = new List<Invoice>();
        foreach (DataRow r in dt.Rows)
            list.Add(MapInvoice(r));
        return list;
    }

    private static Invoice MapInvoice(DataRow row) => new Invoice
    {
        Id = Convert.ToInt32(row["Id"]),
        InvoiceNumber = row.Table.Columns.Contains("InvoiceNumber") ? row["InvoiceNumber"]?.ToString() : "",
        ClientId = row.Table.Columns.Contains("ClientId") && row["ClientId"] != DBNull.Value ? Convert.ToInt32(row["ClientId"]) : 0,
        InvoiceDate = row.Table.Columns.Contains("InvoiceDate") ? row["InvoiceDate"]?.ToString() : "",
        TotalAmount = row.Table.Columns.Contains("TotalAmount") && row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0,
        PaidAmount = row.Table.Columns.Contains("PaidAmount") && row["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["PaidAmount"]) : 0,
        Notes = row.Table.Columns.Contains("Notes") ? row["Notes"]?.ToString() : "",
        Status = row.Table.Columns.Contains("Status") ? row["Status"]?.ToString() : "",
        Type = row.Table.Columns.Contains("Type") ? row["Type"]?.ToString() : "",
        ClientName = row.Table.Columns.Contains("ClientName") ? row["ClientName"]?.ToString() : ""
    };

    // ================== InvoiceItems ==================
    public static int InsertInvoiceItem(InvoiceItem it)
    {
        ExecuteNonQuery(@"INSERT INTO InvoiceItems (InvoiceId, ItemId, ItemCode, ItemName, Quantity, UnitPrice, Discount)
                         VALUES (@inv,@item,@code,@name,@qty,@price,@disc);",
            new SQLiteParameter("@inv", it.InvoiceId),
            new SQLiteParameter("@item", it.ItemId),
            new SQLiteParameter("@code", it.ItemCode ?? ""),
            new SQLiteParameter("@name", it.ItemName ?? ""),
            new SQLiteParameter("@qty", it.Quantity),
            new SQLiteParameter("@price", it.UnitPrice),
            new SQLiteParameter("@disc", it.Discount));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void DeleteInvoiceItem(int id) => ExecuteNonQuery("DELETE FROM InvoiceItems WHERE Id=@Id;", new SQLiteParameter("@Id", id));

    // ================== Returns ==================
    public static int InsertReturn(Return ret)
    {
        ExecuteNonQuery(@"INSERT INTO Returns (ReturnNumber, ClientId, InvoiceId, ReturnDate, TotalAmount, Notes)
                         VALUES (@num,@client,@invoice,@date,@total,@notes);",
            new SQLiteParameter("@num", ret.ReturnNumber),
            new SQLiteParameter("@client", ret.ClientId),
            new SQLiteParameter("@invoice", ret.InvoiceId),
            new SQLiteParameter("@date", ret.ReturnDate),
            new SQLiteParameter("@total", ret.TotalAmount),
            new SQLiteParameter("@notes", ret.Notes));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void DeleteReturn(int id)
    {
        ExecuteNonQuery("DELETE FROM ReturnItems WHERE ReturnId=@Id;", new SQLiteParameter("@Id", id));
        ExecuteNonQuery("DELETE FROM Returns WHERE Id=@Id;", new SQLiteParameter("@Id", id));
    }

    // ================== ReturnItems ==================
    public static int InsertReturnItem(ReturnItem it)
    {
        ExecuteNonQuery(@"INSERT INTO ReturnItems (ReturnId, ItemId, ItemCode, ItemName, Quantity, UnitPrice)
                         VALUES (@rid,@iid,@code,@name,@qty,@price);",
            new SQLiteParameter("@rid", it.ReturnId),
            new SQLiteParameter("@iid", it.ItemId),
            new SQLiteParameter("@code", it.ItemCode),
            new SQLiteParameter("@name", it.ItemName),
            new SQLiteParameter("@qty", it.Quantity),
            new SQLiteParameter("@price", it.UnitPrice));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    // ================== Utility / Reset ==================
    public static void ResetDatabase()
    {
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
        InitializeDatabase();
    }

    internal static void DeleteClient(Client selected)
    {
        throw new NotImplementedException();
    }

    internal static Invoice? GetInvoiceById(int invoiceId)
    {
        throw new NotImplementedException();
    }

    public static List<Category> GetCategories()
    {
        var categories = new List<Category>();

        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            string query = "SELECT CategoryID, Code, Family, SubFamily, SubSubFamily, SubSubSubFamily, SubSubSubSubFamily FROM Categories";

            using (var cmd = new SQLiteCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryID = reader.GetInt32(0),
                        Code = reader.GetString(1),
                        Family = reader.IsDBNull(2) ? null : reader.GetString(2),
                        SubFamily = reader.IsDBNull(3) ? null : reader.GetString(3),
                        SubSubFamily = reader.IsDBNull(4) ? null : reader.GetString(4),
                        SubSubSubFamily = reader.IsDBNull(5) ? null : reader.GetString(5),
                        SubSubSubSubFamily = reader.IsDBNull(6) ? null : reader.GetString(6),
                    });
                }
            }
        }

        return categories;
    }

    internal static void InsertUser(ETAG_ERP.Views.User newUser)
    {
        throw new NotImplementedException();
    }

    internal static void UpdateUser(ETAG_ERP.Views.User editingUser)
    {
        throw new NotImplementedException();
    }

    internal static object ExecuteQuery(string v, Dictionary<string, object> dictionary)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Run scalar query (returns single value)
    /// </summary>
    public static object ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parameters != null)
                {
                    foreach (var p in parameters)
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                }

                return cmd.ExecuteScalar();
            }
        }
    }
    internal static System.Data.DataTable ExecuteQuery(string query)
    {
        var dt = new System.Data.DataTable();

        using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new System.Data.SQLite.SQLiteCommand(query, conn))
            {
                using (var adapter = new System.Data.SQLite.SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
        }

        return dt;
    }

    // ✅ حذف صنف من قاعدة البيانات
    internal static void DeleteItem(object itemID)
    {
        try
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Items WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", itemID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ خطأ أثناء حذف الصنف: " + ex.Message);
        }
    }
    // ✅ إضافة تصنيف جديد إلى قاعدة البيانات
    public static int InsertCategory(ETAG_ERP.Models.Category category)
    {
        if (category == null)
            return 0; // لأننا بنرجع int مش bool

        try
        {
            int rowsAffected = 0;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                INSERT INTO Categories (Level1, Level2, Level3, Level4, Level5, Code)
                VALUES (@L1, @L2, @L3, @L4, @L5, @Code)";
                    cmd.Parameters.AddWithValue("@L1", category.Level1 ?? "");
                    cmd.Parameters.AddWithValue("@L2", category.Level2 ?? "");
                    cmd.Parameters.AddWithValue("@L3", category.Level3 ?? "");
                    cmd.Parameters.AddWithValue("@L4", category.Level4 ?? "");
                    cmd.Parameters.AddWithValue("@L5", category.Level5 ?? "");
                    cmd.Parameters.AddWithValue("@Code", category.Code ?? "");

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            // ✅ إضافة التصنيف إلى بيانات السيدينج (في الذاكرة)
            var seedList = CategorySeeder.GetSeedData();

            // تجنب التكرار لو الكود موجود فعلاً
            if (!seedList.Any(x => x.Code == category.Code))
            {
                seedList.Add(new CategorySeedData(
                    category.Level1,
                    category.Level2,
                    category.Level3,
                    category.Level4,
                    category.Level5,
                    category.Code
                ));
            }

            return rowsAffected; // بيرجع عدد الصفوف المتأثرة
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ خطأ أثناء إضافة التصنيف: " + ex.Message);
            return 0;
        }
    }

    // ✅ دالة مبسطة تستدعي InsertCategory وتتعامل مع الأخطاء داخليًا
    internal static void AddCategory(ETAG_ERP.Models.Category category)
    {
        try
        {
            int inserted = InsertCategory(category);
            if (inserted <= 0)
                Console.WriteLine("⚠️ فشل في إضافة التصنيف داخل AddCategory.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ خطأ أثناء AddCategory: " + ex.Message);
        }
    }

    // ✅ دالة لزرع التصنيفات الأساسية (Seed) عند أول تشغيل فقط
    public static void SeedCategoriesIfEmpty()
    {
        try
        {
            var existing = GetAllCategories();
            if (existing != null && existing.Count > 0)
                return;

            var seedData = ETAG_ERP.Helpers.CategorySeeder.GetSeedData();
            foreach (var seed in seedData)
            {
                var category = new ETAG_ERP.Models.Category
                {
                    Level1 = seed.Level1,
                    Level2 = seed.Level2,
                    Level3 = seed.Level3,
                    Level4 = seed.Level4,
                    Level5 = seed.Level5,
                    Code = seed.Code
                };

                int success = InsertCategory(category);
                if (success <= 0)
                    Console.WriteLine($"⚠️ فشل في إدخال التصنيف {category.Level1}-{category.Level2}");
            }

            Console.WriteLine("✅ تم تنفيذ السيدنج للتصنيفات بنجاح.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ خطأ أثناء عملية السيدنج: " + ex.Message);
        }
    }


}
