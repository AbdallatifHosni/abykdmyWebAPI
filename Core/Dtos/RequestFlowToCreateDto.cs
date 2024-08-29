using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class RequestFlowToCreateDto
    {
        public int RequestId { get; set; }
        [Required(ErrorMessage = "يجب ادخال الاسئلة المسبقة لهذه الخدمة")]
        public int RequirementId { get; set; }
        public int? EmployeeId { get; set; }
        public int Status { get; set; } = 1;
    }
}
