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
    public class ReturnService
    {
        public int AddReturn(Return r)
        {
            var sql = @"INSERT INTO Returns (ReturnNumber, Date, ClientId, Total, Notes)
                        VALUES (@ReturnNumber, @Date, @ClientId, @Total, @Notes);
                        SELECT last_insert_rowid();";
            var idObj = DatabaseHelper.ExecuteScalar(sql,
                new System.Data.SQLite.SQLiteParameter("@ReturnNumber", (object?)r.ReturnNumber ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Date", r.Date.ToString("o")),
                new System.Data.SQLite.SQLiteParameter("@ClientId", (object?)r.ClientId ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Total", r.Total),
                new System.Data.SQLite.SQLiteParameter("@Notes", (object?)r.Notes ?? DBNull.Value)
            );
            return Convert.ToInt32(idObj);
        }

        public void UpdateReturn(Return r)
        {
            var sql = @"UPDATE Returns SET ReturnNumber=@ReturnNumber, Date=@Date, ClientId=@ClientId, Total=@Total, Notes=@Notes WHERE Id=@Id";
            DatabaseHelper.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@ReturnNumber", (object?)r.ReturnNumber ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Date", r.Date.ToString("o")),
                new System.Data.SQLite.SQLiteParameter("@ClientId", (object?)r.ClientId ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Total", r.Total),
                new System.Data.SQLite.SQLiteParameter("@Notes", (object?)r.Notes ?? DBNull.Value),
                new System.Data.SQLite.SQLiteParameter("@Id", r.Id)
            );
        }

        public void DeleteReturn(int id) => DatabaseHelper.ExecuteNonQuery("DELETE FROM Returns WHERE Id=@Id", new System.Data.SQLite.SQLiteParameter("@Id", id));

        public List<Return> GetAllReturns()
        {
            var list = new List<Return>();
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Returns ORDER BY Date DESC");
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Return
                {
                    Id = Convert.ToInt32(r["Id"]),
                    ReturnNumber = r["ReturnNumber"] == DBNull.Value ? null : r["ReturnNumber"].ToString(),
                    Date = DateTime.TryParse(r["Date"].ToString(), out var d) ? d : DateTime.Now,
                    ClientId = (int)(r["ClientId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ClientId"])),
                    Total = r["Total"] == DBNull.Value ? 0 : Convert.ToDouble(r["Total"]),
                    Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString()
                });
            }
            return list;
        }
    }
}
