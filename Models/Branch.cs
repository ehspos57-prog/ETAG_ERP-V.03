namespace ETAG_ERP.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Location { get; set; } = "";
        public bool IsActive { get; set; }
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "";
    }
}
