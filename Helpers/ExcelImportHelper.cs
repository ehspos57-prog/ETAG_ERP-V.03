using ExcelDataReader;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace ETAG_ERP.Helpers
{
    public static class ExcelImportHelper
    {
        static ExcelImportHelper()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public static DataTable ReadExcelSheet(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                return result.Tables[0];
            }
        }

        public static void ImportItems(string filePath)
        {
            var table = ReadExcelSheet(filePath);
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                for (int i = 1; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"
                            INSERT INTO Items 
                            (Id, ItemCode, Name, Price1, Price2, Price3, PurchasePrice, Quantity, Unit, CategoryId, Barcode, CreatedAt, UpdatedAt)
                            VALUES
                            (@Id, @ItemCode, @Name, @Price1, @Price2, @Price3, @PurchasePrice, @Quantity, @Unit, @CategoryId, @Barcode, @CreatedAt, @UpdatedAt)";

                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                        cmd.Parameters.AddWithValue("@ItemCode", row[0]?.ToString());
                        cmd.Parameters.AddWithValue("@Name", row[1]?.ToString());
                        cmd.Parameters.AddWithValue("@Price1", Convert.ToDouble(row[2]));
                        cmd.Parameters.AddWithValue("@Price2", Convert.ToDouble(row[3]));
                        cmd.Parameters.AddWithValue("@Price3", Convert.ToDouble(row[4]));
                        cmd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDouble(row[5]));
                        cmd.Parameters.AddWithValue("@Quantity", Convert.ToDouble(row[6]));
                        cmd.Parameters.AddWithValue("@Unit", row[7]?.ToString());
                        cmd.Parameters.AddWithValue("@CategoryId", Convert.ToInt32(row[8]));
                        cmd.Parameters.AddWithValue("@Barcode", row[9]?.ToString());
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void ImportClients(string filePath)
        {
            var table = ReadExcelSheet(filePath);
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                for (int i = 1; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"
                            INSERT INTO Clients (Name, Phone, Email, Address, Notes, Balance, CreatedAt, UpdatedAt)
                            VALUES (@Name, @Phone, @Email, @Address, @Notes, @Balance, @CreatedAt, @UpdatedAt)";

                        cmd.Parameters.AddWithValue("@Name", row[0]?.ToString());
                        cmd.Parameters.AddWithValue("@Phone", row[1]?.ToString());
                        cmd.Parameters.AddWithValue("@Email", row[2]?.ToString());
                        cmd.Parameters.AddWithValue("@Address", row[3]?.ToString());
                        cmd.Parameters.AddWithValue("@Notes", row[4]?.ToString());
                        cmd.Parameters.AddWithValue("@Balance", Convert.ToDouble(row[5]));
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
