using AbyKhedma.Entities;
using Core.Models;

namespace Core.Dtos
{
    public class ChatMessageToCreateDto
    {
        public int RequestFlowId { get; set; }
        public int FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public int IsRead { get; set; } = 0;
        public string? Url { get; set; }
        public List<string>? Urls { get; set; }
        public string? UrlPublicId { get; set; }
        public int? AnswerId { get; set; }
        public List<MessageModel>? Messages { get; set; } 
    }
    
}
