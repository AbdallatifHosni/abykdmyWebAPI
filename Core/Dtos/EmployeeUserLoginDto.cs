using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class EmployeeUserLoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
