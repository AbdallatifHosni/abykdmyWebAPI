

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class SubAnswerRequirement : BaseEntity
    {
        public int ServiceID { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
        public int StepOrder { get; set; }
        public int RequirementType { get; set; } = 1;
        public string? Url { get; set; }
        public List<Answer>? Answers { get; set; }
        public virtual Service Service { get; set; }
        public int AnswerId { get; set; }
        [ForeignKey("AnswerId")]
        public Answer Answer { get; set; }

    }
}
