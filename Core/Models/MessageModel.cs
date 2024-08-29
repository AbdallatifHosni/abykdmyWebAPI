namespace Core.Models
{
    public class MessageModel
    {
        public int? Id { get; set; }
        public int MessageType { get; set; }
        public string Content { get; set; }
        public int? ChatMessageId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
