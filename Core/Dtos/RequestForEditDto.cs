using AbyKhedma.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Dtos
{
    public class RequestForEditDto
    {
        public int ServiceId { get; set; }
        public int RequesterId { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public int? Status { get; set; } = (int)RequestStatus.CreateNewRequest;
        public string? Description { get; set; } 
    }
}
