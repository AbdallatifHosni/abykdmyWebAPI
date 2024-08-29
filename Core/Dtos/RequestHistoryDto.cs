using AbyKhedma.Entities;
using Core.Models;

namespace AbyKhedma.Dtos
{
    public class RequestHistoryDto
    {
        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public int? RequesterId { get; set; }
        public int? StatusId { get; set; }
        public int? RequirementId { get; set; }
        public UserDto? Requester { get; set; }
        public UserDto? Employee { get; set; }
        public int? AnswerId { get; set; }
        public ServiceModel? Service { get; set; }
        public StatusLookup? StatusLookup { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<RequestAudit>  RequestStatusHistoryList { get; set; }
    }
}
