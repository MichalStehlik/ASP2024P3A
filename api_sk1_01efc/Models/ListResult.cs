namespace api_sk1_01efc.Models
{
    public class ListResult<T>
    {
        public required List<T> Data { get; set; }
        public int Total {  get; set; }
    }
}
