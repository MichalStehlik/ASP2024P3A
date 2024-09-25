using System.Text.Json.Serialization;

namespace api_sk1_01efc.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public required string Text { get; set; }
        public int UserId { get; set; } // 
        [JsonIgnore]
        public Article? Article { get; set; } // navigation property
        public required int ArticleId { get; set; } // cizí klíč
    }
}
