using AbyKhedma.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class RequestFlow : BaseEntity
    {
        public int? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public virtual Request Request { get; set; }
        public int? RequirementId { get; set; }
        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual User User { get; set; }
        public int Status { get; set; }
    }
}
