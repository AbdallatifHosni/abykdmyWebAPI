using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class RequesterUserLoginDto
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        [Required]
        public string DeviceToken { get; set; }
    }
}
