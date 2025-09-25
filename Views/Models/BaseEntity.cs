using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETAG_ERP.Models
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
