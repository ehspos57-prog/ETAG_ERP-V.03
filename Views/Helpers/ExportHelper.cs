using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETAG_ERP.Helpers
{
    public static class ExportHelper
    {
        [Obsolete]
        public static void ExportDataTableToExcel(DataTable dt, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");

                worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}
