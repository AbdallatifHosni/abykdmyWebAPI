using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class RequestAudit : BaseEntity
    {
        public int RequestId { get; set; }
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual StatusLookup  Status { get; set; }
        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public User? User { get; set; }
    }
}
