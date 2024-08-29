namespace AbyKhedma.Dtos
{
    public class UserDto
    {
        public int? Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string FullName { get; set; }
        public string? PhotoUrl { get; set; }
        public bool? IsActive { get; set; }
        public string Role { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string? UrlPublicId { get; set; }
        public int? Subscription { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? PaymentReceiptUrl { get; set; }
        public string? PaymentReceiptUrlPublicId { get; set; }
    }
}
