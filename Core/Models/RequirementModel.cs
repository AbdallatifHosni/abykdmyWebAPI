using Core.Dtos;

namespace Core.Models
{
    public class RequirementModel
    {
        public int Id { get; set; }
        public int ServiceID { get; set; }
        public string Description { get; set; }
        public int StepOrder { get; set; }
        public int RequirementType { get; set; } = 1;
        public List<AnswerDto> Answers { get; set; }
    }
}
