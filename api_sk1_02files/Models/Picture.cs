namespace api_sk1_02files.Models
{
    public class Picture
    {
        public required Guid ItemId { get; set; }
        public Item? Item { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public required byte[] Content { get; set; }
    }
}
