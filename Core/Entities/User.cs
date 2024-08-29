
using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(25)]
        public string? PhoneNumber { get; set; }
        [MaxLength(25)]
        public string? UserName { get; set; }
        [MaxLength(50)]
        public string? Email { get; set; }
        [MaxLength(25)]
        public string? FirstName { get; set; }
        [MaxLength(25)]
        public string? FamilyName { get; set; }
        [MaxLength(60)]
        public string? FullName { get; set; }
        //[MaxLength(60)]
        
        //public string? SearchableFullName { get; set; }
        [MaxLength(400)]
        public string? PhotoUrl { get; set; }
        public bool? IsActive { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        [MaxLength(15)]
        public string? Role { get; set; }
        [MaxLength(10)]
        public string? PassCode { get; set; }
        public DateTime  PassCodeExpiry { get; set; }
        public DateTime? LastLoginTime { get; set; }
        [MaxLength(30)]
        public string? UrlPublicId { get; set; }
        [MaxLength(100)]
        public string? RefreshToken { get; set; }
        public int? Subscription { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public decimal?  PaymentAmount { get; set; }
        [MaxLength(400)]
        public string?  PaymentReceiptUrl { get; set; } 
        [MaxLength(30)]
        public string? PaymentReceiptUrlPublicId { get; set; }
        [MaxLength(255)]
        public string? DeviceToken { get; set; }
        public int? PasscodeResendTryCount { get; set; } = 0;
        public int? LoginFailureCount { get; set; }=0;
        public string? MsgProvider { get; set; } = "phone";

        //private string RefineFullName()
        //{
        //    if(FullName == null)
        //    {
        //        return string.Empty;
        //    }
        //    return FullName.Replace(" ", "");
        //}
    }
}
