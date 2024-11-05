namespace api_sk1_02files.InputModels
{
    public class FileListVM
    {
        public Guid ItemId { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public int Size { get; set; }
        public required DateTime Created { get; set; }
        public byte[]? Thumbnail { get; set; } = null;
    }
}
