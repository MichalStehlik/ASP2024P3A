namespace api_sk1_02files.Models
{
    public class Item
    {
        public Guid ItemId { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required DateTime Created { get; set; }
    }
}
