
using System;

namespace API.Data.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int RecipiantId { get; set; }
        public string RecipiantUsername { get; set; }
        public AppUser Recipiant { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;
        public bool SenderDeleted { get; set; }
        public bool RecipiantDeleted { get; set; }
    }
}
