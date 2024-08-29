using AbyKhedma.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class ChatMessageModel
    {
        public int Id { get; set; }
        public int RequestFlowId { get; set; }
        public RequirementModel? Requirement { get; set; }
        public int FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int IsRead { get; set; } = 0;
        public string? Url { get; set; }
        public string? UrlPublicId { get; set; }
        public int? AnswerId { get; set; }
        public Answer? Answer { get; set; }
        public List<MessageModel>? Messages { get; set; }

    }
}
