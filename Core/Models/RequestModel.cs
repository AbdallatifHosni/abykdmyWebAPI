using AbyKhedma.Dtos;
using AbyKhedma.Entities;
using Core.Models;

namespace AbyKhedma.Core.Models
{
    public class RequestModel
    {
        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public int? RequesterId { get; set; }
        public int? StatusId { get; set; }
        public int? RequirementId { get; set; }
        public UserDto?  Requester { get; set; }
        public UserDto? Employee { get; set; }
        public int? AnswerId { get; set; }
        public ServiceModel?   Service { get; set; }
        public StatusLookup?   StatusLookup { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool? IsShowedByTheEmployee { get; set; } = false;
        public bool? IsShowedByTheRequester { get; set; } = false;
        public bool? IsArchived { get; set; } = false;
    }
}
