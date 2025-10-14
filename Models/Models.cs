using ETAG_ERP.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETAG_ERP.Models
{



    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";

    }

    // =======================
    // المستخدمين والموظفين
    // =======================

    public class EmployeeLite
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Job { get; set; } = "";
        public string Phone { get; set; } = "";

    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string FullName { get; set; } = "";
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "";
        public int? BranchId { get; set; } // Nullable
        public string Password { get; set; }
        public string Role { get; set; }
        public List<Permission> Permissions { get; set; } = new List<Permission>();

    }

    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "";

        // علاقات
        public List<Route> Routes { get; set; } = new List<Route>();
        public List<TaskSchedule> Tasks { get; set; } = new List<TaskSchedule>();
    }

    public class Route
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string StartLocation { get; set; } = "";
        public string EndLocation { get; set; } = "";
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public List<string> Stops { get; set; } = new List<string>();
    }

    public class TaskSchedule
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string TaskName { get; set; } = "";
        public string ClientName { get; set; } = "";
        public DateTime ScheduledDate { get; set; }
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Location { get; set; } = "";
        public string Status { get; set; } = "مجدول"; // جاري - مكتمل - مؤجل
    }

    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsAdmin { get; set; }
    }

    // =======================
    // الحسابات
    // =======================
    public class AccountLite
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public double OpeningBalance { get; set; }
        public string Description { get; set; } = "";
    }

    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "";
        public decimal? OpeningBalance { get; set; }
        public string? Description { get; set; }
    }

    public class LedgerEntry
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string RefNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string? ClientName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Account { get; set; }
        public string? Notes { get; set; }
        public string Reference { get; set; } = "";
        public string Category { get; set; } = ""; // مشروع / مصروف / عميل
        public int? AccountId { get; set; }
        public string? ProjectName { get; set; }
    }

    public class TransactionModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? AccountId { get; set; }
        public Account? Account { get; set; }
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public string? InvoiceNumber { get; set; }
    }

    // =======================
    // الأصناف والمنتجات
    // =======================
    public class Category
    {
        public int Id { get; set; }
        public int? ParentId { get; set; } = 0; // Nullable
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "";
        public string? Description { get; set; }
        public int CategoryID { get; set; }
        public string? Family { get; set; }
        public string? SubFamily { get; set; }
        public string? SubSubFamily { get; set; }
        public string? SubSubSubFamily { get; set; }
        public string? SubSubSubSubFamily { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Level1 { get; set; }
        public string? Level2 { get; set; }
        public string? Level3 { get; set; }
        public string? Level4 { get; set; }
        public string? Level5 { get; set; }
        public string? Code { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = "";
        public string Code { get; set; } = "";
        public string ItemCode { get; set; } = "";
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 { get; set; }
        public int StockQuantity { get; set; }
        public int MinStock { get; set; }
        public string? Unit { get; set; }
        public string? Barcode { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Discount { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public string? CategoryPath { get; set; }
        public List<string> Categories { get; set; } = new List<string>();

        // Cat1..Cat5
        public string? Cat1 { get; set; }
        public string? Cat2 { get; set; }
        public string? Cat3 { get; set; }
        public string? Cat4 { get; set; }
        public string? Cat5 { get; set; }
    }

    // =======================
    // العملاء
    // =======================
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public decimal Balance { get; set; }
        public string? TaxCard { get; set; }
        public string? CommercialRecord { get; set; }
        public string? ResponsibleEngineer { get; set; }
        public string? Fax { get; set; }
        public string? BusinessField { get; set; }
        public string? EngineerName { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public string? Evaluator { get; set; }
        public string? CompanyEvaluation { get; set; }
        public bool RatingGood { get; set; }
        public bool RatingAverage { get; set; }
        public bool RatingPoor { get; set; }

        public List<ContactPerson> Contacts { get; set; } = new List<ContactPerson>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public List<Return> Returns { get; set; } = new List<Return>();
        public List<PriceOffer> PriceOffers { get; set; } = new List<PriceOffer>();
    }

    public class ContactPerson
    {
        public string Name { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // =======================
    // الفواتير
    // =======================
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? ClientName { get; set; }
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "غير مدفوعة";
        public string Type { get; set; } = "مبيعات";
        public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public int? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public int? ItemId { get; set; }
        public Item? Item { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total => (UnitPrice * Quantity) - Discount;
    }

    public class InvoiceLine
    {
        public int Id { get; set; }
        public int? InvoiceId { get; set; }
        public int? ItemId { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int? ClientId { get; set; }
        public string? Notes { get; set; }

        public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    }

    // =======================
    // المرتجعات
    // =======================
    public class Return
    {
        public int Id { get; set; }
        public string ReturnNumber { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public int? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public DateTime ReturnDate { get; set; }
        public List<ReturnItem> Items { get; set; } = new List<ReturnItem>();
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public double Total { get; set; }
    }

    public class ReturnItem
    {
        public int Id { get; set; }
        public int? ReturnId { get; set; }
        public Return? Return { get; set; }
        public int? ItemId { get; set; }
        public Item? Item { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ReturnNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int? ClientId { get; set; }
        public double Total { get; set; }
        public string? Notes { get; set; }
    }

    // =======================
    // عروض الأسعار
    // =======================
    public class PriceOffer
    {
        public int Id { get; set; }
        public string OfferNumber { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime OfferDate { get; set; }
        public List<PriceOfferItem> Items { get; set; } = new List<PriceOfferItem>();
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class PriceOfferItem
    {
        public int Id { get; set; }
        public int? OfferId { get; set; }
        public PriceOffer? Offer { get; set; }
        public int? ItemId { get; set; }
        public Item? Item { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }

    // =======================
    // المشتريات
    // =======================
    public class Purchase
    {
        public int Id { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public Employee? Supplier { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public double Total { get; set; }
        public string? Notes { get; set; }
        public decimal? PaidAmount { get; set; }
    }

    public class PurchaseItem
    {
        public int Id { get; set; }
        public int? PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        public int? ItemId { get; set; }
        public Item? Item { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public string? PurchaseNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Supplier { get; set; }
        public double Total { get; set; }
        public string? Notes { get; set; }
    }

    // =======================
    // المساعدات
    // =======================
    public class QuoteItem : INotifyPropertyChanged
    {
        private string _name;
        private decimal _quantity;
        private decimal _unitPrice;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public decimal Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set { _unitPrice = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        public decimal Total => Quantity * UnitPrice;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public class Safe
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "";
    }

    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public string? ExpenseType { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string? Category { get; set; }
        public string? Username { get; set; }

    }

}
public class Category
{
    public int Id { get; set; }
    public int ParentId { get; set; } = 0; // صفر للـ root
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
    public object Description { get; internal set; }
    public bool IsActive { get; internal set; }
    public int CategoryID { get; internal set; }
    public string Code { get; internal set; }
    public string? Family { get; internal set; }
    public string? SubFamily { get; internal set; }
    public string? SubSubFamily { get; internal set; }
    public string? SubSubSubFamily { get; internal set; }
    public string? SubSubSubSubFamily { get; internal set; }
    public string? Level1 { get; internal set; }
    public string? Level2 { get; internal set; }
    public string? Level3 { get; internal set; }
    public string? Level4 { get; internal set; }
    public string? Level5 { get; internal set; }
}
public class LedgerEntry : BaseEntity
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string RefNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string? ClientName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string? Account { get; set; }
    public string? Notes { get; set; }
    public string Reference { get; set; } = "";
    public string Category { get; set; } = ""; // مشروع / مصروف / عميل
    public int AccountId { get; set; }
    public string? ProjectName { get; set; }
}
public class TransactionModel : BaseEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? AccountId { get; set; }
    public int? GetInvoiceNumber() => invoiceNumber;
    public void SetInvoiceNumber(int? value) => invoiceNumber = value;
    public Account? Account { get; set; }
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public string? InvoiceNumber { get; set; }

    private int? invoiceNumber;
}

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string JobTitle { get; set; } = "";
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Notes { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";

    // علاقات
    public List<Route> Routes { get; set; } = new List<Route>();
    public List<TaskSchedule> Tasks { get; set; } = new List<TaskSchedule>();

    public static implicit operator Employee?(string? v)
    {
        throw new NotImplementedException();
    }
}

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
    public decimal? OpeningBalance { get; set; }
    public string? Description { get; set; }
}
// =======================
// العملاء
// =======================
public class Client : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public decimal Balance { get; set; }
    public string? TaxCard { get; set; }
    public string? CommercialRecord { get; set; }
    public string? ResponsibleEngineer { get; set; }
    public string? Fax { get; set; }
    public string? BusinessField { get; set; }


    public string? EngineerName { get; set; }
    public DateTime? EvaluationDate { get; set; }
    public string? Evaluator { get; set; }
    public string? CompanyEvaluation { get; set; }
    public bool RatingGood { get; set; }
    public bool RatingAverage { get; set; }
    public bool RatingPoor { get; set; }

    public List<ContactPerson> Contacts { get; set; } = new List<ContactPerson>();
    public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    public List<Return> Returns { get; set; } = new List<Return>();
    public List<PriceOffer> PriceOffers { get; set; } = new List<PriceOffer>();
    public int ClientID { get; internal set; }
}

public class ContactPerson
{
    public string Name { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public object UserName { get; internal set; }
    public object Password { get; internal set; }
}

// =======================
// الأصناف والمنتجات
// =======================
public class Item : BaseEntity
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty; // تمت إضافتها
    public int Quantity { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal Price1 { get; set; }
    public decimal Price2 { get; set; }
    public decimal Price3 { get; set; }
    public int StockQuantity { get; set; }
    public int MinStock { get; set; } // تمت إضافتها
    public string? Unit { get; set; }
    public string? Barcode { get; set; }
    public decimal? Tax { get; set; }
    public decimal? Discount { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public string Name { get; set; } = string.Empty;
    public double SellPrice { get; set; }
    public string? CategoryPath { get; set; }
    public List<string> Categories { get; set; } = new List<string>();

    // Cat1..Cat5
    public string? Cat1 { get; set; }
    public string? Cat2 { get; set; }
    public string? Cat3 { get; set; }
    public string? Cat4 { get; set; }
    public string? Cat5 { get; set; }
    public string? Category { get; internal set; }
    public int ItemID { get; internal set; }
}

// =======================
// الفواتير
// =======================
public class Invoice : BaseEntity
{
    internal int totalAmount;

    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public DateTime Date { get; set; } // تمت إضافتها
    public string ClientName { get; set; } = string.Empty; // تمت إضافتها
    public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "غير مدفوعة";
    public string Type { get; set; } = "مبيعات";
    public object InvoiceDate { get; internal set; }
    public string? Description { get; internal set; }
    public string? Username { get; internal set; }
    public string? SupplierName { get; internal set; }
    public double Total { get; set; }
    public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}

public class InvoiceItem : BaseEntity
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total => (UnitPrice * Quantity) - Discount;
}

// =======================
// المرتجعات
// =======================
public class Return : BaseEntity
{
    public int Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public DateTime ReturnDate { get; set; }
    public List<ReturnItem> Items { get; set; } = new List<ReturnItem>();
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public double Total { get; set; }

}

public class ReturnItem : BaseEntity
{
    public int Id { get; set; }
    public int ReturnId { get; set; }
    public Return? Return { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ReturnNumber { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public int? ClientId { get; set; }
    public double Total { get; set; }
    public string? Notes { get; set; }

}

// =======================
// عروض الأسعار
// =======================
public class PriceOffer : BaseEntity
{
    public int Id { get; set; }
    public string OfferNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public DateTime OfferDate { get; set; }
    public List<PriceOfferItem> Items { get; set; } = new List<PriceOfferItem>();
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
}

public class QuoteItem : INotifyPropertyChanged
{
    private string _name;
    private decimal _quantity;
    private decimal _unitPrice;

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public decimal Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set { _unitPrice = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
    }

    public decimal Total => Quantity * UnitPrice;

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
public class Purchase : BaseEntity
{
    public int Id { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public Employee? Supplier { get; set; }
    public DateTime PurchaseDate { get; set; }
    public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public double Total { get; set; }
    public string? Notes { get; set; }
    public object PaidAmount { get; internal set; }
}

public class PurchaseItem : BaseEntity
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public string? PurchaseNumber { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string? Supplier { get; set; }
    public double Total { get; set; }
    public string? Notes { get; set; }
}
public class PriceOfferItem : BaseEntity
{
    public int Id { get; set; }
    public int OfferId { get; set; }
    public PriceOffer? Offer { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => UnitPrice * Quantity;
}