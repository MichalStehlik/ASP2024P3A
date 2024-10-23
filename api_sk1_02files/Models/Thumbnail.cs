namespace api_sk1_02files.Models
{
    public class Thumbnail
    {
        public Guid ThumbnailId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; }
        public byte[] Content { get; set; }
    }
}
