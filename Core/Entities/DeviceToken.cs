using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class DeviceToken: BaseEntity
    {
        public int UserId { get; set; }
        [MaxLength(255)]
        public string Token { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
