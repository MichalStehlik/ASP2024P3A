namespace api_sk1_02files.InputModels
{
    public class Listing<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }
        public required List<T> Items { get; set; }
    }
}
