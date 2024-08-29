using Core.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class Answer : BaseEntity
    {
        [MaxLength(150)]
        public string Description { get; set; }
        public bool? IsTerminationAnswer { get; set; } = false;
        [MaxLength(200)]
        public string? TerminationStatement { get; set; }
        public int? HasSubAnswerRequirement { get; set; } = 0;
        public int?   SubAnswerRequirementId { get; set; }
        public  virtual SubAnswerRequirement?  SubAnswerRequirement { get; set; }
    }
   
}
