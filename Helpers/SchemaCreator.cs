using System.Data.SQLite;
using System.IO;

namespace ETAG_ERP.Helpers
{
    public static class SchemaCreator
    {
        public static void CreateDatabaseAndTables(string dbFile = "etag_erp.db")
        {
            if (File.Exists(dbFile))
            {
                // قاعدة البيانات موجودة بالفعل، لا حاجة لإعادة إنشائها
                return;
            }

            // إنشاء ملف قاعدة البيانات
            SQLiteConnection.CreateFile(dbFile);

            using var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;");
            conn.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Items (
                    Id TEXT PRIMARY KEY,
                    ItemCode TEXT,
                    Name TEXT,
                    Price1 REAL,
                    Price2 REAL,
                    Price3 REAL,
                    PriceWithVat REAL,
                    PurchasePrice REAL,
                    Barcode TEXT,
                    Quantity REAL,
                    MinStock REAL,
                    Weight REAL,
                    Unit TEXT,
                    CategoryId INTEGER,
                    SubCategoryId INTEGER,
                    CreatedAt TEXT,
                    UpdatedAt TEXT
                );

                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    ParentId INTEGER,
                    FOREIGN KEY (ParentId) REFERENCES Categories(Id)
                );

                CREATE TABLE IF NOT EXISTS Clients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Phone TEXT,
                    Email TEXT,
                    Address TEXT,
                    Notes TEXT,
                    Balance REAL DEFAULT 0,
                    CreatedAt TEXT,
                    UpdatedAt TEXT
                );

                CREATE TABLE IF NOT EXISTS Invoices (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER,
                    TotalAmount REAL,
                    Discount REAL,
                    NetAmount REAL,
                    PaidAmount REAL,
                    RemainingAmount REAL,
                    InvoiceDate TEXT,
                    Notes TEXT,
                    CreatedAt TEXT,
                    UpdatedAt TEXT,
                    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
                );

                CREATE TABLE IF NOT EXISTS InvoiceItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId INTEGER,
                    ItemId TEXT,
                    Quantity REAL,
                    UnitPrice REAL,
                    Total REAL,
                    FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id),
                    FOREIGN KEY (ItemId) REFERENCES Items(Id)
                );

                CREATE TABLE IF NOT EXISTS Returns (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId INTEGER,
                    ReturnDate TEXT,
                    Notes TEXT,
                    CreatedAt TEXT,
                    FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id)
                );

                CREATE TABLE IF NOT EXISTS ReturnItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReturnId INTEGER,
                    ItemId TEXT,
                    Quantity REAL,
                    UnitPrice REAL,
                    Total REAL,
                    FOREIGN KEY (ReturnId) REFERENCES Returns(Id),
                    FOREIGN KEY (ItemId) REFERENCES Items(Id)
                );

                CREATE TABLE IF NOT EXISTS Purchases (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SupplierName TEXT,
                    PurchaseDate TEXT,
                    TotalAmount REAL,
                    Notes TEXT,
                    CreatedAt TEXT
                );

                CREATE TABLE IF NOT EXISTS PurchaseItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PurchaseId INTEGER,
                    ItemId TEXT,
                    Quantity REAL,
                    UnitPrice REAL,
                    Total REAL,
                    FOREIGN KEY (PurchaseId) REFERENCES Purchases(Id),
                    FOREIGN KEY (ItemId) REFERENCES Items(Id)
                );

                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Salt TEXT NOT NULL,
                    IsAdmin INTEGER DEFAULT 0,
                    CanSeePurchasePrice INTEGER DEFAULT 0,
                    CanExport INTEGER DEFAULT 0,
                    CreatedAt TEXT
                );

                CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    ExpenseDate TEXT,
                    Notes TEXT
                );

                CREATE TABLE IF NOT EXISTS Ledger (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    EntryDate TEXT,
                    Description TEXT,
                    Debit REAL DEFAULT 0,
                    Credit REAL DEFAULT 0,
                    Balance REAL
                );

                CREATE TABLE IF NOT EXISTS PriceOffers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    CreatedBy TEXT,
                    ClientId INTEGER,
                    Notes TEXT,
                    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
                );

                CREATE TABLE IF NOT EXISTS PriceOfferItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OfferId INTEGER,
                    ItemId TEXT,
                    Quantity REAL NOT NULL,
                    Price REAL NOT NULL,
                    Total REAL,
                    FOREIGN KEY (OfferId) REFERENCES PriceOffers(Id),
                    FOREIGN KEY (ItemId) REFERENCES Items(Id)
                );
            ";

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
    }
}
