using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class ChatMessage : BaseEntity
    {
        public int RequestFlowId { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
 
        [MaxLength(400)]
        public string? Url { get; set; }
        [MaxLength(50)]
        public string? UrlPublicId { get; set; }
        public int IsRead { get; set; } = 0;
        public int? AnswerId { get; set; }
        [ForeignKey("AnswerId")]
        public Answer? Answer { get; set; }
        public virtual ICollection<Message>?  Messages { get; set; }

    }
}
