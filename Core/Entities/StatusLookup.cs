using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Entities
{
    public class StatusLookup
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        public int StatusType { get; set; }
    }
}
