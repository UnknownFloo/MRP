namespace MRP.Models
{
    public class User
    {
        public int uuid { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<int> favorites { get; set; } = new();
    }
}