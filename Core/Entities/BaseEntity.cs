using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Entities
{
 public abstract class BaseEntity
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool? Deleted { get; set; }=false;
    }
}
