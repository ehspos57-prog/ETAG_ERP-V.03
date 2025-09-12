using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace ETAG_ERP.Services
{
    public class InvoiceService
    {
        public int AddInvoice(Invoice invoice)
        {
            var sql = @"INSERT INTO Invoices (InvoiceNumber, Date, ClientId, Total, Notes)
                        VALUES (@InvoiceNumber, @Date, @ClientId, @Total, @Notes);
                        SELECT last_insert_rowid();";
            var idObj = DatabaseHelper.ExecuteScalar(sql,
                new SQLiteParameter("@InvoiceNumber", (object?)invoice.InvoiceNumber ?? DBNull.Value),
                new SQLiteParameter("@Date", invoice.Date.ToString("o")),
                new SQLiteParameter("@ClientId", (object?)invoice.ClientId ?? DBNull.Value),
                new SQLiteParameter("@Total", invoice.Total),
                new SQLiteParameter("@Notes", (object?)invoice.Notes ?? DBNull.Value)
            );

            var invoiceId = Convert.ToInt32(idObj);

            // احفظ خطوط الفاتورة
            foreach (var line in invoice.Lines)
            {
                var sqlLine = @"INSERT INTO InvoiceLines (InvoiceId, ItemId, Quantity, UnitPrice, Total)
                                VALUES (@InvoiceId, @ItemId, @Quantity, @UnitPrice, @Total);";
                DatabaseHelper.ExecuteNonQuery(sqlLine,
                    new SQLiteParameter("@InvoiceId", invoiceId),
                    new SQLiteParameter("@ItemId", line.ItemId),
                    new SQLiteParameter("@Quantity", line.Quantity),
                    new SQLiteParameter("@UnitPrice", line.UnitPrice),
                    new SQLiteParameter("@Total", line.Total)
                );
            }

            return invoiceId;
        }

        public void UpdateInvoice(Invoice invoice)
        {
            var sql = @"UPDATE Invoices SET InvoiceNumber=@InvoiceNumber, Date=@Date, ClientId=@ClientId, Total=@Total, Notes=@Notes WHERE Id=@Id";
            DatabaseHelper.ExecuteNonQuery(sql,
                new SQLiteParameter("@InvoiceNumber", (object?)invoice.InvoiceNumber ?? DBNull.Value),
                new SQLiteParameter("@Date", invoice.Date.ToString("o")),
                new SQLiteParameter("@ClientId", (object?)invoice.ClientId ?? DBNull.Value),
                new SQLiteParameter("@Total", invoice.Total),
                new SQLiteParameter("@Notes", (object?)invoice.Notes ?? DBNull.Value),
                new SQLiteParameter("@Id", invoice.Id)
            );

            // لتبسيط المثال: نحذف خطوط الفاتورة القديمة ونضيف الحالية
            DatabaseHelper.ExecuteNonQuery("DELETE FROM InvoiceLines WHERE InvoiceId=@InvoiceId", new SQLiteParameter("@InvoiceId", invoice.Id));
            foreach (var line in invoice.Lines)
            {
                DatabaseHelper.ExecuteNonQuery(@"INSERT INTO InvoiceLines (InvoiceId, ItemId, Quantity, UnitPrice, Total)
                                                VALUES (@InvoiceId, @ItemId, @Quantity, @UnitPrice, @Total);",
                    new SQLiteParameter("@InvoiceId", invoice.Id),
                    new SQLiteParameter("@ItemId", line.ItemId),
                    new SQLiteParameter("@Quantity", line.Quantity),
                    new SQLiteParameter("@UnitPrice", line.UnitPrice),
                    new SQLiteParameter("@Total", line.Total)
                );
            }
        }

        public void DeleteInvoice(int id)
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM InvoiceLines WHERE InvoiceId=@InvoiceId", new SQLiteParameter("@InvoiceId", id));
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Invoices WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public List<Invoice> GetAllInvoices()
        {
            var list = new List<Invoice>();
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Invoices ORDER BY Date DESC");
            foreach (DataRow r in dt.Rows)
            {
                var inv = new Invoice
                {
                    Id = Convert.ToInt32(r["Id"]),
                    InvoiceNumber = r["InvoiceNumber"] == DBNull.Value ? null : r["InvoiceNumber"].ToString(),
                    Date = DateTime.TryParse(r["Date"].ToString(), out var d) ? d : DateTime.Now,
                    ClientId = (int)(r["ClientId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ClientId"])),
                    Total = r["Total"] == DBNull.Value ? 0 : Convert.ToDouble(r["Total"]),
                    Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString()
                };

                // أحضِر خطوط الفاتورة
                var dtLines = DatabaseHelper.GetDataTable("SELECT * FROM InvoiceLines WHERE InvoiceId=@InvoiceId", new SQLiteParameter("@InvoiceId", inv.Id));
                foreach (DataRow lr in dtLines.Rows)
                {
                    inv.Lines.Add(new InvoiceLine
                    {
                        Id = Convert.ToInt32(lr["Id"]),
                        InvoiceId = Convert.ToInt32(lr["InvoiceId"]),
                        ItemId = Convert.ToInt32(lr["ItemId"]),
                        Quantity = lr["Quantity"] == DBNull.Value ? 0 : Convert.ToDouble(lr["Quantity"]),
                        UnitPrice = lr["UnitPrice"] == DBNull.Value ? 0 : Convert.ToDouble(lr["UnitPrice"]),
                        Total = lr["Total"] == DBNull.Value ? 0 : Convert.ToDouble(lr["Total"])
                    });
                }

                list.Add(inv);
            }
            return list;
        }

        public Invoice? GetInvoiceById(int id)
        {
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Invoices WHERE Id=@Id", new SQLiteParameter("@Id", id));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            var inv = new Invoice
            {
                Id = Convert.ToInt32(r["Id"]),
                InvoiceNumber = r["InvoiceNumber"] == DBNull.Value ? null : r["InvoiceNumber"].ToString(),
                Date = DateTime.TryParse(r["Date"].ToString(), out var d) ? d : DateTime.Now,
                ClientId = (int)(r["ClientId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ClientId"])),
                Total = r["Total"] == DBNull.Value ? 0 : Convert.ToDouble(r["Total"]),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString()
            };

            var dtLines = DatabaseHelper.GetDataTable("SELECT * FROM InvoiceLines WHERE InvoiceId=@InvoiceId", new SQLiteParameter("@InvoiceId", inv.Id));
            foreach (DataRow lr in dtLines.Rows)
            {
                inv.Lines.Add(new InvoiceLine
                {
                    Id = Convert.ToInt32(lr["Id"]),
                    InvoiceId = Convert.ToInt32(lr["InvoiceId"]),
                    ItemId = Convert.ToInt32(lr["ItemId"]),
                    Quantity = lr["Quantity"] == DBNull.Value ? 0 : Convert.ToDouble(lr["Quantity"]),
                    UnitPrice = lr["UnitPrice"] == DBNull.Value ? 0 : Convert.ToDouble(lr["UnitPrice"]),
                    Total = lr["Total"] == DBNull.Value ? 0 : Convert.ToDouble(lr["Total"])
                });
            }

            return inv;
        }
    }
}
