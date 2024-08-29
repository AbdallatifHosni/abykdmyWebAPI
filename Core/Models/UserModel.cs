

using AbyKhedma.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Core.Models
{
    public class UserModel  
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int IsActive { get; set; }
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
