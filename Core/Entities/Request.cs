using AbyKhedma.Core.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class Request : BaseEntity
    {
        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        public int RequesterId { get; set; }
        public int StatusId { get; set; } = (int)RequestStatus.CreateNewRequest;
        [ForeignKey("StatusId")]
        public virtual StatusLookup? StatusLookup { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public DateTime? IsLockedToDateTime { get; set; } = DateTime.UtcNow;
        [MaxLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        [ForeignKey("RequesterId")]
        public virtual User Requester { get; set; }
        [ForeignKey("AssignedEmployeeId")]
        public virtual User? Employee { get; set; }
        public bool? IsShowedByTheEmployee   { get; set; } 
        public bool? IsShowedByTheRequester { get; set; }
        public bool? IsArchived { get; set; } = false;

    }
}
