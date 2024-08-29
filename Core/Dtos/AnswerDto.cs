using AbyKhedma.Entities;

namespace Core.Dtos
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public int RequirementId { get; set; }
        public string? Description { get; set; }
        public bool? IsTerminationAnswer { get; set; } = false;
        public string? TerminationStatement { get; set; }
        public int? HasSubAnswerRequirement { get; set; } = 0;
        public SubAnswerRequirementDto? SubAnswerRequirement { get; set; }
    }
}
