public class User
{
    public int ID { get; set; } // لازم يكون فيه
    public string Username { get; set; }
    public string UserName { get; internal set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public bool IsAdmin { get; set; }
    public string Permissions { get; set; }  // أو أي نوع مناسب لو permissions معمولة بصيغة مختلفة
    public object UserID { get; internal set; }
    public string ImagePath { get; internal set; }
}
