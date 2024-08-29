using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class ServiceToCreateDto
    {
        [MaxLength(150)]
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int OpeningStatementId { get; set; }
        public int ClosingStatementId { get; set; }
        public bool? IsSystem { get; set; } = false;
    }
}
