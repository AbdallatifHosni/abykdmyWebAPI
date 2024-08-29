

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class Requirement : BaseEntity
    {
        public int ServiceID { get; set; }
        [MaxLength(160)]
        public string Description { get; set; }
        public int  StepOrder { get; set; }
        public int RequirementType { get; set; } = 1;
        [MaxLength(450)]
        public string?  Url { get; set; }
        public bool? IsSystem { get; set; } = false;
        public List<Answer>? Answers { get; set; }
        public virtual Service Service { get; set; }
    }
}
