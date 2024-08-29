using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class Message : BaseEntity
    {
        public int MessageType { get; set; }/*1,2,3,4*/
        [MaxLength(400)]
        public string Content { get; set; }
        public int ChatMessageId { get; set; }
        [ForeignKey("ChatMessageId")]
        public virtual ChatMessage?  ChatMessage { get; set; }
    }
}
