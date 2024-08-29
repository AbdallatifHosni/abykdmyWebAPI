using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Entities
{
    public class Statement : BaseEntity
    {
        [MaxLength(200)]
        public string StatementText { get; set; }
        public int StatemenType { get; set; }
    }
}
