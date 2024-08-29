namespace Core.Dtos
{
    public class RequestSearchDto
    {
        public int? RequestId { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public string? RequesterPhoneNumber { get; set; }
        public string? RequesterName { get; set; }
        public int? StatusId { get; set; }

    }
}
