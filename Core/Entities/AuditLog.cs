using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime ActivityTime { get; set; }
    }
}
