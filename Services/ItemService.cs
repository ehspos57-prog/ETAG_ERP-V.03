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
    public class ItemService
    {
        public int AddItem(Item item)
        {
            var sql = @"INSERT INTO Items (Code, Name, PurchasePrice, SellPrice, Price1, Price2, Price3, StockQuantity, CategoryPath)
                        VALUES (@Code,@Name,@PurchasePrice,@SellPrice,@Price1,@Price2,@Price3,@StockQuantity,@CategoryPath);
                        SELECT last_insert_rowid();";
            var idObj = DatabaseHelper.ExecuteScalar(sql,
                new SQLiteParameter("@Code", (object?)item.Code ?? DBNull.Value),
                new SQLiteParameter("@Name", item.Name),
                new SQLiteParameter("@PurchasePrice", item.PurchasePrice),
                new SQLiteParameter("@SellPrice", item.SellPrice),
                new SQLiteParameter("@Price1", item.Price1),
                new SQLiteParameter("@Price2", item.Price2),
                new SQLiteParameter("@Price3", item.Price3),
                new SQLiteParameter("@StockQuantity", item.StockQuantity),
                new SQLiteParameter("@CategoryPath", (object?)item.CategoryPath ?? DBNull.Value)
            );
            return Convert.ToInt32(idObj);
        }

        public void UpdateItem(Item item)
        {
            var sql = @"UPDATE Items SET Code=@Code, Name=@Name, PurchasePrice=@PurchasePrice, SellPrice=@SellPrice, Price1=@Price1, Price2=@Price2, Price3=@Price3, StockQuantity=@StockQuantity, CategoryPath=@CategoryPath WHERE Id=@Id";
            DatabaseHelper.ExecuteNonQuery(sql,
                new SQLiteParameter("@Code", (object?)item.Code ?? DBNull.Value),
                new SQLiteParameter("@Name", item.Name),
                new SQLiteParameter("@PurchasePrice", item.PurchasePrice),
                new SQLiteParameter("@SellPrice", item.SellPrice),
                new SQLiteParameter("@Price1", item.Price1),
                new SQLiteParameter("@Price2", item.Price2),
                new SQLiteParameter("@Price3", item.Price3),
                new SQLiteParameter("@StockQuantity", item.StockQuantity),
                new SQLiteParameter("@CategoryPath", (object?)item.CategoryPath ?? DBNull.Value),
                new SQLiteParameter("@Id", item.Id)
            );
        }

        public void DeleteItem(int id)
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Items WHERE Id=@Id", new SQLiteParameter("@Id", id));
        }

        public List<Item> GetAllItems()
        {
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Items ORDER BY Name");
            var list = new List<Item>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Item
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Code = r["Code"] == DBNull.Value ? null : r["Code"].ToString(),
                    Name = r["Name"].ToString() ?? string.Empty,
                    PurchasePrice = (decimal)(r["PurchasePrice"] == DBNull.Value ? 0 : Convert.ToDouble(r["PurchasePrice"])),
                    SellPrice = r["SellPrice"] == DBNull.Value ? 0 : Convert.ToDouble(r["SellPrice"]),
                    Price1 = (decimal)(r["Price1"] == DBNull.Value ? 0 : Convert.ToDouble(r["Price1"])),
                    Price2 = (decimal)(r["Price2"] == DBNull.Value ? 0 : Convert.ToDouble(r["Price2"])),
                    Price3 = (decimal)(r["Price3"] == DBNull.Value ? 0 : Convert.ToDouble(r["Price3"])),
                    StockQuantity = (int)(r["StockQuantity"] == DBNull.Value ? 0 : Convert.ToDouble(r["StockQuantity"])),
                    CategoryPath = r["CategoryPath"] == DBNull.Value ? null : r["CategoryPath"].ToString()
                });
            }
            return list;
        }

        public Item? GetItemById(int id)
        {
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Items WHERE Id=@Id", new SQLiteParameter("@Id", id));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Item
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"] == DBNull.Value ? null : r["Code"].ToString(),
                Name = r["Name"].ToString() ?? string.Empty,
                PurchasePrice = (decimal)(r["PurchasePrice"] == DBNull.Value ? 0 : Convert.ToDouble(r["PurchasePrice"])),
                SellPrice = r["SellPrice"] == DBNull.Value ? 0 : Convert.ToDouble(r["SellPrice"]),
                Price1 = (decimal)(r["Price1"] == DBNull.Value ? 0 : Convert.ToDouble(r["Price1"])),
                Price2 = (decimal)(r["Price2"] == DBNull.Value ? 0 : Convert.ToDouble(r["Price2"])),
                Price3 = (decimal)(r["Price3"] == DBNull.Value ? 0 : Convert.ToDouble(r["Price3"])),
                StockQuantity = (int)(r["StockQuantity"] == DBNull.Value ? 0 : Convert.ToDouble(r["StockQuantity"])),
                CategoryPath = r["CategoryPath"] == DBNull.Value ? null : r["CategoryPath"].ToString()
            };
        }
    }
}
