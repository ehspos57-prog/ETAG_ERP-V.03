using ETAG_ERP.Models;
using System.Data;
using System.Data.SQLite;
using System.IO;

public static class DatabaseHelper
{
    private static string _dbPath = "ETAG_ERP.db";
    private static readonly string _connectionString = $"Data Source={_dbPath};Version=3;";

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

        // ================== الجداول ==================
        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Phone TEXT, Email TEXT, Address TEXT, Notes TEXT,
                    Balance REAL, TaxCard TEXT, CommercialRecord TEXT,
                    ResponsibleEngineer TEXT, Fax TEXT,
                    BusinessField TEXT, EngineerName TEXT,
                    EvaluationDate TEXT, Evaluator TEXT,
                    CompanyEvaluation TEXT,
                    RatingGood INTEGER, RatingAverage INTEGER, RatingPoor INTEGER
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT, PasswordHash TEXT, FullName TEXT, IsAdmin INTEGER
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Permissions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Description TEXT
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Branches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL, Address TEXT, Phone TEXT
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT, JobTitle TEXT, Salary REAL, HireDate TEXT,
                    Phone TEXT, Email TEXT, Notes TEXT, Role TEXT
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Accounts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Type TEXT, Balance REAL
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Safes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, Balance REAL
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT, ParentID INTEGER
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Items (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ItemName TEXT, Code TEXT, Quantity INTEGER, SellingPrice REAL, PurchasePrice REAL,
                    Price1 REAL, Price2 REAL, Price3 REAL, Cat1 TEXT, Cat2 TEXT, Cat3 TEXT,
                    Cat4 TEXT, Cat5 TEXT, MinStock INTEGER, Description TEXT,
                    Unit TEXT, Barcode TEXT, Tax REAL, Discount REAL, ImagePath TEXT
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Invoices (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceNumber TEXT, ClientId INTEGER, InvoiceDate TEXT,
                    TotalAmount REAL, PaidAmount REAL, Notes TEXT, Status TEXT, Type TEXT, ClientName TEXT
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS InvoiceItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId INTEGER, ItemId INTEGER,
                    ItemCode TEXT, ItemName TEXT,
                    Quantity REAL, UnitPrice REAL, Discount REAL
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Returns (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReturnNumber TEXT, ClientId INTEGER, InvoiceId INTEGER,
                    ReturnDate TEXT, TotalAmount REAL, Notes TEXT
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ReturnItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReturnId INTEGER, ItemId INTEGER,
                    ItemCode TEXT, ItemName TEXT,
                    Quantity REAL, UnitPrice REAL
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS PriceOffers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OfferNumber TEXT, ClientId INTEGER, OfferDate TEXT, TotalAmount REAL, Notes TEXT
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS PriceOfferItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OfferId INTEGER, ItemId INTEGER,
                    ItemCode TEXT, ItemName TEXT,
                    Quantity REAL, UnitPrice REAL
                );";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ExpenseType TEXT, Amount REAL, ExpenseDate TEXT, Description TEXT, Category TEXT
                );";
        cmd.ExecuteNonQuery();

        // ================== الأعمدة الناقصة ==================
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

        conn.Close();
    }

    /// <summary>
    /// يتأكد من وجود الأعمدة ويضيف أي ناقص
    /// </summary>
    private static void EnsureColumns(SQLiteConnection conn, string tableName, Dictionary<string, string> expectedColumns)
    {
        var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using (var pragma = new SQLiteCommand($"PRAGMA table_info([{tableName}]);", conn))
        using (var reader = pragma.ExecuteReader())
        {
            while (reader.Read())
            {
                var name = reader["name"]?.ToString();
                if (!string.IsNullOrEmpty(name)) existing.Add(name);
            }
        }

        foreach (var kv in expectedColumns)
        {
            if (!existing.Contains(kv.Key))
            {
                using var alter = new SQLiteCommand($"ALTER TABLE [{tableName}] ADD COLUMN [{kv.Key}] {kv.Value};", conn);
                alter.ExecuteNonQuery();
            }
        }
    }

    // ================== Execute Helpers ==================
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
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            using var cmd = new SQLiteCommand(sql, conn);
            if (parameters?.Length > 0) cmd.Parameters.AddRange(parameters);
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
    // ================== Extra Helpers ==================

    public static object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SQLiteCommand(sql, conn);
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    public static void SaveBackgroundImage(string imagePath)
    {
        ExecuteNonQuery("CREATE TABLE IF NOT EXISTS Settings (Key TEXT PRIMARY KEY, Value TEXT);");
        ExecuteNonQuery("INSERT OR REPLACE INTO Settings (Key, Value) VALUES ('BackgroundImage', @path);",
            new SQLiteParameter("@path", imagePath));
    }

    public static int InsertClient(Client client)
    {
        string sql = @"INSERT INTO Clients (Name, Phone, Email, Address, Notes, Balance) 
                   VALUES (@Name, @Phone, @Email, @Address, @Notes, @Balance);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Name", client.Name),
            new SQLiteParameter("@Phone", client.Phone),
            new SQLiteParameter("@Email", client.Email),
            new SQLiteParameter("@Address", client.Address),
            new SQLiteParameter("@Notes", client.Notes),
            new SQLiteParameter("@Balance", client.Balance));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateClient(Client client)
    {
        string sql = @"UPDATE Clients SET Name=@Name, Phone=@Phone, Email=@Email, Address=@Address,
                   Notes=@Notes, Balance=@Balance WHERE Id=@Id;";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Name", client.Name),
            new SQLiteParameter("@Phone", client.Phone),
            new SQLiteParameter("@Email", client.Email),
            new SQLiteParameter("@Address", client.Address),
            new SQLiteParameter("@Notes", client.Notes),
            new SQLiteParameter("@Balance", client.Balance),
            new SQLiteParameter("@Id", client.Id));
    }

    public static void DeleteClient(int clientId)
    {
        ExecuteNonQuery("DELETE FROM Clients WHERE Id=@Id;",
            new SQLiteParameter("@Id", clientId));
    }
    public static void DeleteClient(Client client)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        DeleteClient(client.Id);
    }

    // -------- Mapping helpers --------
    private static Client MapClient(DataRow row) => new Client
    {
        Id = Convert.ToInt32(row["Id"]),
        Name = row["Name"]?.ToString() ?? "",
        Phone = row["Phone"]?.ToString(),
        Email = row["Email"]?.ToString(),
        Address = row["Address"]?.ToString(),
        Notes = row["Notes"]?.ToString(),
        Balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0
    };

    public static int InsertExpense(Expense expense)
    {
        string sql = @"INSERT INTO Expenses (ExpenseType, Amount, ExpenseDate, Description, Category)
                   VALUES (@ExpenseType, @Amount, @ExpenseDate, @Description, @Category);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@ExpenseType", expense.ExpenseType),
            new SQLiteParameter("@Amount", expense.Amount),
            new SQLiteParameter("@ExpenseDate", expense.ExpenseDate),
            new SQLiteParameter("@Description", expense.Description),
            new SQLiteParameter("@Category", expense.Category));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateExpense(Expense expense)
    {
        string sql = @"UPDATE Expenses SET ExpenseType=@ExpenseType, Amount=@Amount, 
                   ExpenseDate=@ExpenseDate, Description=@Description, Category=@Category 
                   WHERE Id=@Id;";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@ExpenseType", expense.ExpenseType),
            new SQLiteParameter("@Amount", expense.Amount),
            new SQLiteParameter("@ExpenseDate", expense.ExpenseDate),
            new SQLiteParameter("@Description", expense.Description),
            new SQLiteParameter("@Category", expense.Category),
            new SQLiteParameter("@Id", expense.Id));
    }

    public static int InsertItem(Item item)
    {
        string sql = @"INSERT INTO Items (ItemName, Code, Quantity, SellingPrice, PurchasePrice,
                   Price1, Price2, Price3, Cat1, Cat2, Cat3, Cat4, Cat5, MinStock, Description,
                   Unit, Barcode, Tax, Discount, ImagePath)
                   VALUES (@ItemName,@Code,@Quantity,@SellingPrice,@PurchasePrice,
                   @Price1,@Price2,@Price3,@Cat1,@Cat2,@Cat3,@Cat4,@Cat5,@MinStock,@Description,
                   @Unit,@Barcode,@Tax,@Discount,@ImagePath);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@ItemName", item.ItemName),
            new SQLiteParameter("@Code", item.Code),
            new SQLiteParameter("@Quantity", item.Quantity),
            new SQLiteParameter("@SellingPrice", item.SellingPrice),
            new SQLiteParameter("@PurchasePrice", item.PurchasePrice),
            new SQLiteParameter("@Price1", item.Price1),
            new SQLiteParameter("@Price2", item.Price2),
            new SQLiteParameter("@Price3", item.Price3),
            new SQLiteParameter("@Cat1", item.Cat1),
            new SQLiteParameter("@Cat2", item.Cat2),
            new SQLiteParameter("@Cat3", item.Cat3),
            new SQLiteParameter("@Cat4", item.Cat4),
            new SQLiteParameter("@Cat5", item.Cat5),
            new SQLiteParameter("@MinStock", item.MinStock),
            new SQLiteParameter("@Description", item.Description),
            new SQLiteParameter("@Unit", item.Unit),
            new SQLiteParameter("@Barcode", item.Barcode),
            new SQLiteParameter("@Tax", item.Tax),
            new SQLiteParameter("@Discount", item.Discount),
            new SQLiteParameter("@ImagePath", item.ImagePath));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    private static Item MapItem(DataRow row) => new Item
    {
        Id = Convert.ToInt32(row["Id"]),
        ItemName = row["ItemName"]?.ToString() ?? "",
        Code = row["Code"]?.ToString(),
        Quantity = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0,
        SellingPrice = row["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(row["SellingPrice"]) : 0,
        PurchasePrice = row["PurchasePrice"] != DBNull.Value ? Convert.ToDecimal(row["PurchasePrice"]) : 0,
        Price1 = row["Price1"] != DBNull.Value ? Convert.ToDecimal(row["Price1"]) : 0,
        Price2 = row["Price2"] != DBNull.Value ? Convert.ToDecimal(row["Price2"]) : 0,
        Price3 = row["Price3"] != DBNull.Value ? Convert.ToDecimal(row["Price3"]) : 0,
        Cat1 = row["Cat1"]?.ToString(),
        Cat2 = row["Cat2"]?.ToString(),
        Cat3 = row["Cat3"]?.ToString(),
        Cat4 = row["Cat4"]?.ToString(),
        Cat5 = row["Cat5"]?.ToString(),
        MinStock = row["MinStock"] != DBNull.Value ? Convert.ToInt32(row["MinStock"]) : 0,
        Description = row["Description"]?.ToString(),
        Unit = row["Unit"]?.ToString(),
        Barcode = row["Barcode"]?.ToString(),
        Tax = row["Tax"] != DBNull.Value ? Convert.ToDecimal(row["Tax"]) : 0,
        Discount = row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0,
        ImagePath = row["ImagePath"]?.ToString()
    };

    public static int InsertUser(User user)
    {
        string sql = @"INSERT INTO Users (Username, PasswordHash, FullName, IsAdmin) 
                   VALUES (@Username,@PasswordHash,@FullName,@IsAdmin);";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Username", user.Username),
            new SQLiteParameter("@PasswordHash", user.Password),
            new SQLiteParameter("@FullName", user.FullName),
            new SQLiteParameter("@IsAdmin", user.IsAdmin ? 1 : 0));
        return Convert.ToInt32(ExecuteScalar("SELECT last_insert_rowid();"));
    }

    public static void UpdateUser(User user)
    {
        string sql = @"UPDATE Users SET Username=@Username, PasswordHash=@PasswordHash, 
                   FullName=@FullName, IsAdmin=@IsAdmin WHERE Id=@Id;";
        ExecuteNonQuery(sql,
            new SQLiteParameter("@Username", user.Username),
            new SQLiteParameter("@PasswordHash", user.Password),
            new SQLiteParameter("@FullName", user.FullName),
            new SQLiteParameter("@IsAdmin", user.IsAdmin ? 1 : 0),
            new SQLiteParameter("@Id", user.Id));
    }

    private static Invoice MapInvoice(DataRow row) => new Invoice
    {
        Id = Convert.ToInt32(row["Id"]),
        InvoiceNumber = row["InvoiceNumber"]?.ToString(),
        ClientId = row["ClientId"] != DBNull.Value ? Convert.ToInt32(row["ClientId"]) : 0,
        InvoiceDate = row["InvoiceDate"]?.ToString(),
        TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0,
        PaidAmount = row["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["PaidAmount"]) : 0,
        Notes = row["Notes"]?.ToString(),
        Status = row["Status"]?.ToString(),
        Type = row["Type"]?.ToString(),
        ClientName = row["ClientName"]?.ToString()
    };


    // -------- Replacements --------
    public static List<Client> GetAllClients()
    {
        var dt = GetDataTable("SELECT * FROM Clients;");
        var list = new List<Client>();
        foreach (DataRow row in dt.Rows)
            list.Add(MapClient(row));
        return list;
    }

    public static List<Item> GetAllItems()
    {
        var dt = GetDataTable("SELECT * FROM Items;");
        var list = new List<Item>();
        foreach (DataRow row in dt.Rows)
            list.Add(MapItem(row));
        return list;
    }

    public static List<Invoice> GetAllInvoices()
    {
        var dt = GetDataTable("SELECT * FROM Invoices;");
        var list = new List<Invoice>();
        foreach (DataRow row in dt.Rows)
            list.Add(MapInvoice(row));
        return list;
    }

    public static Invoice? GetInvoiceById(int id)
    {
        var dt = GetDataTable("SELECT * FROM Invoices WHERE Id=@Id;", new SQLiteParameter("@Id", id));
        return dt.Rows.Count > 0 ? MapInvoice(dt.Rows[0]) : null;
    }

    public static void ResetDatabase()
    {
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
        InitializeDatabase();
    }

}

