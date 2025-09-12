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

namespace ETAG_ERP.Services
{
    public class PurchaseService
    {
        public int AddPurchase(Purchase p)
        {
            var sql = @"INSERT INTO Purchases (PurchaseNumber, Date, Supplier, Total, Notes)
                        VALUES (@PurchaseNumber, @Date, @Supplier, @Total, @Notes);
                        SELECT last_insert_rowid();";
            var idObj = DatabaseHelper.ExecuteScalar(sql,
                new System.Data.SQLite.SQLiteParameter("@PurchaseNumber", (object?)p.PurchaseNumber ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Date", p.Date.ToString("o")),
                new System.Data.SQLite.SQLiteParameter("@Supplier", (object?)p.Supplier ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Total", p.Total),
                new System.Data.SQLite.SQLiteParameter("@Notes", (object?)p.Notes ?? DBNull.Value)
            );
            return Convert.ToInt32(idObj);
        }

        public void UpdatePurchase(Purchase p)
        {
            var sql = @"UPDATE Purchases SET PurchaseNumber=@PurchaseNumber, Date=@Date, Supplier=@Supplier, Total=@Total, Notes=@Notes WHERE Id=@Id";
            DatabaseHelper.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@PurchaseNumber", (object?)p.PurchaseNumber ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Date", p.Date.ToString("o")),
                new System.Data.SQLite.SQLiteParameter("@Supplier", (object?)p.Supplier ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Total", p.Total),
                new System.Data.SQLite.SQLiteParameter("@Notes", (object?)p.Notes ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Id", p.Id)
            );
        }

        public void DeletePurchase(int id) => DatabaseHelper.ExecuteNonQuery("DELETE FROM Purchases WHERE Id=@Id", new System.Data.SQLite.SQLiteParameter("@Id", id));

        public List<Purchase> GetAllPurchases()
        {
            var list = new List<Purchase>();
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Purchases ORDER BY Date DESC");
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Purchase
                {
                    Id = Convert.ToInt32(r["Id"]),
                    PurchaseNumber = r["PurchaseNumber"] == DBNull.Value ? null : r["PurchaseNumber"].ToString(),
                    Date = DateTime.TryParse(r["Date"].ToString(), out var d) ? d : DateTime.Now,
                    Supplier = r["Supplier"] == DBNull.Value ? null : r["Supplier"].ToString(),
                    Total = r["Total"] == DBNull.Value ? 0 : Convert.ToDouble(r["Total"]),
                    Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString()
                });
            }
            return list;
        }
    }
}
