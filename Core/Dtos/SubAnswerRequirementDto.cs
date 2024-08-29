namespace Core.Dtos
{
    public class SubAnswerRequirementDto
    {
        public int ServiceID { get; set; }
        public string Description { get; set; }
        public int StepOrder { get; set; }
        public int RequirementType { get; set; } = 1;
        public List<AnswerDto>? Answers { get; set; }
    }
    public class SubAnswerRequirementToCreateDto
    {
        public int AnswerId { get; set; }
        public int ServiceID { get; set; }
        public string Description { get; set; }
        public int StepOrder { get; set; }
        public int RequirementType { get; set; } = 1;
        public List<AnswerDto>? Answers { get; set; }
    }
}