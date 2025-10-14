using System.Data.SQLite;

namespace ETAG_ERP.Helpers
{
    public static class DatabaseInitializer
    {
        public static void InitializeDatabase()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            // Branches
            var createBranches = @"
                CREATE TABLE IF NOT EXISTS Branches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Address TEXT,
                    Phone TEXT
                );";
            using (var cmd = new SQLiteCommand(createBranches, conn)) cmd.ExecuteNonQuery();

            // Clients
            var createClients = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Phone TEXT,
                    Email TEXT,
                    Address TEXT,
                    Notes TEXT
                );";
            using (var cmd = new SQLiteCommand(createClients, conn)) cmd.ExecuteNonQuery();

            // Items
            var createItems = @"
                CREATE TABLE IF NOT EXISTS Items (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Code TEXT,
                    Name TEXT NOT NULL,
                    PurchasePrice REAL,
                    SellPrice REAL,
                    Price1 REAL,
                    Price2 REAL,
                    Price3 REAL,
                    StockQuantity REAL,
                    CategoryPath TEXT
                );";
            using (var cmd = new SQLiteCommand(createItems, conn)) cmd.ExecuteNonQuery();

            // Invoices
            var createInvoices = @"
                CREATE TABLE IF NOT EXISTS Invoices (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceNumber TEXT,
                    Date TEXT,
                    ClientId INTEGER,
                    Total REAL,
                    Notes TEXT,
                    FOREIGN KEY(ClientId) REFERENCES Clients(Id)
                );";
            using (var cmd = new SQLiteCommand(createInvoices, conn)) cmd.ExecuteNonQuery();

            // Invoice lines
            var createInvoiceLines = @"
                CREATE TABLE IF NOT EXISTS InvoiceLines (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId INTEGER,
                    ItemId INTEGER,
                    Quantity REAL,
                    UnitPrice REAL,
                    Total REAL,
                    FOREIGN KEY(InvoiceId) REFERENCES Invoices(Id),
                    FOREIGN KEY(ItemId) REFERENCES Items(Id)
                );";
            using (var cmd = new SQLiteCommand(createInvoiceLines, conn)) cmd.ExecuteNonQuery();

            // Purchases
            var createPurchases = @"
                CREATE TABLE IF NOT EXISTS Purchases (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PurchaseNumber TEXT,
                    Date TEXT,
                    Supplier TEXT,
                    Total REAL,
                    Notes TEXT
                );";
            using (var cmd = new SQLiteCommand(createPurchases, conn)) cmd.ExecuteNonQuery();

            // Returns
            var createReturns = @"
                CREATE TABLE IF NOT EXISTS Returns (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReturnNumber TEXT,
                    Date TEXT,
                    ClientId INTEGER,
                    Total REAL,
                    Notes TEXT,
                    FOREIGN KEY(ClientId) REFERENCES Clients(Id)
                );";
            using (var cmd = new SQLiteCommand(createReturns, conn)) cmd.ExecuteNonQuery();

            // Expenses
            var createExpenses = @"
                CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Description TEXT,
                    Amount REAL
                );";
            using (var cmd = new SQLiteCommand(createExpenses, conn)) cmd.ExecuteNonQuery();

            // Ledger (دفتر أستاذ بسيط)
            var createLedger = @"
                CREATE TABLE IF NOT EXISTS Ledger (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Account TEXT,
                    Debit REAL,
                    Credit REAL,
                    Notes TEXT
                );";
            using (var cmd = new SQLiteCommand(createLedger, conn)) cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}

