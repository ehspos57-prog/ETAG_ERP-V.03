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
    public class ClientService
    {
        public int AddClient(Client c)
        {
            var sql = "INSERT INTO Clients (Name, Phone, Email, Address, Notes) VALUES (@Name,@Phone,@Email,@Address,@Notes); SELECT last_insert_rowid();";
            var idObj = DatabaseHelper.ExecuteScalar(sql,
                new SQLiteParameter("@Name", c.Name),
                new SQLiteParameter("@Phone", (object?)c.Phone ?? DBNull.Value),
                new SQLiteParameter("@Email", (object?)c.Email ?? DBNull.Value),
                new SQLiteParameter("@Address", (object?)c.Address ?? DBNull.Value),
                new SQLiteParameter("@Notes", (object?)c.Notes ?? DBNull.Value)
            );
            return Convert.ToInt32(idObj);
        }

        public void UpdateClient(Client c)
        {
            var sql = "UPDATE Clients SET Name=@Name, Phone=@Phone, Email=@Email, Address=@Address, Notes=@Notes WHERE Id=@Id";
            DatabaseHelper.ExecuteNonQuery(sql,
                new SQLiteParameter("@Name", c.Name),
                new SQLiteParameter("@Phone", (object?)c.Phone ?? DBNull.Value),
                new SQLiteParameter("@Email", (object?)c.Email ?? DBNull.Value),
                new SQLiteParameter("@Address", (object?)c.Address ?? DBNull.Value),
                new SQLiteParameter("@Notes", (object?)c.Notes ?? DBNull.Value),
                new SQLiteParameter("@Id", c.Id)
            );
        }

        public void DeleteClient(int id)
        {
            var sql = "DELETE FROM Clients WHERE Id=@Id";
            DatabaseHelper.ExecuteNonQuery(sql, new SQLiteParameter("@Id", id));
        }

        public List<Client> GetAllClients()
        {
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Clients ORDER BY Name");
            var list = new List<Client>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Client
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"].ToString() ?? string.Empty,
                    Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
                    Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
                    Address = r["Address"] == DBNull.Value ? null : r["Address"].ToString(),
                    Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString()
                });
            }
            return list;
        }

        public Client? GetClientById(int id)
        {
            var dt = DatabaseHelper.GetDataTable("SELECT * FROM Clients WHERE Id=@Id", new SQLiteParameter("@Id", id));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Client
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = r["Name"].ToString() ?? string.Empty,
                Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
                Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
                Address = r["Address"] == DBNull.Value ? null : r["Address"].ToString(),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString()
            };
        }
    }
}
