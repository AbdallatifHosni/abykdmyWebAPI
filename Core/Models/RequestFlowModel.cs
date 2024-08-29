using AbyKhedma.Entities;
using Core.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class RequestFlowModel 
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int? RequirementId { get; set; }
        public RequirementModel? Requirement { get; set; }
        public int? EmployeeId { get; set; }
        public int Status { get; set; }
    }
}
