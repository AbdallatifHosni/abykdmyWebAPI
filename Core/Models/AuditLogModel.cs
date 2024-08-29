

using AbyKhedma.Dtos;

namespace Core.Models
{
    public class AuditLogModel
    {
        public int Id { get; set; }
        public   UserDto User { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime ActivityTime { get; set; }
    }
}
