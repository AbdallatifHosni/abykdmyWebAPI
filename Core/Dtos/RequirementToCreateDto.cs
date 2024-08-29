namespace Core.Dtos
{
    public class RequirementToCreateDto
    {

        public int ServiceID { get; set; }
        public string Description { get; set; }
        public int StepOrder { get; set; }
        public int RequirementType { get; set; } = 1;
        public bool? IsSystem { get; set; } = false;
        public List<AnswerDto>? Answers { get; set; }
        
    }
    public class RequirementToEditDto
    {
        public int Id { get; set; }
        public int ServiceID { get; set; }
        public string Description { get; set; }
        public int StepOrder { get; set; }
        public int RequirementType { get; set; } = 1;
        public bool? IsSystem { get; set; } = false;
        public List<AnswerDto>? Answers { get; set; }

    }
    public class AnswerToAddDto
    {
        public int RequirementId { get; set; }
        public string? Description { get; set; }
        public bool? IsTerminationAnswer { get; set; } = false;
        public string? TerminationStatement { get; set; }
        public int? HasSubAnswerRequirement { get; set; } = 0;
        public SubAnswerRequirementDto? SubAnswerRequirement { get; set; }
    }
}
