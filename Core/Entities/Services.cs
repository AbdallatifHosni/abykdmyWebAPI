using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Entities
{
    public class Service : BaseEntity
    {
        public Service()
        {
            Requirements = new List<Requirement>();
        }
        [MaxLength(150)]
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
        public decimal Price { get; set; }
        public bool? IsSystem { get; set; } = false;
        public ICollection<Requirement> Requirements { get; set; }
    }
}
