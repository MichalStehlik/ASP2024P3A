using Microsoft.AspNetCore.Identity;

namespace api_sk1_03identity.Models
{
    public class Note
    {
        public int NoteId { get; set; }
        public required string Text { get; set; }
        public required string UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
