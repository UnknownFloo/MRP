namespace MRP.Models
{
    public class Media
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string media_type { get; set; } = string.Empty;
        public int release_year { get; set; }
        public List<string> genre { get; set; } = new();
        public int age_restriction { get; set; }
        public string? created_by { get; set; }
    }
}