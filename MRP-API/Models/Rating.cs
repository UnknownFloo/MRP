namespace MRP.Models
{
    public class Rating
    {
        public int id { get; set; }
        public int media_id { get; set; }
        public string rating_by { get; set; } = string.Empty;
        public int stars { get; set; }
        public string? comment { get; set; }
        public bool comment_confirm { get; set; }
        public List<string> liked_by { get; set; } = new();
        
    }
}