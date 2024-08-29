using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Dtos
{
    public class RequesterUserForRegisterDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "FirstName length must be greater than 2 characters"), MaxLength(20, ErrorMessage = "FirstName length must be less than 20 characters"),]
        public string FirstName { get; set; }
        public int IsActive { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "FamilyName length must be greater than 2 characters"), MaxLength(20, ErrorMessage = "FamilyName length must be less than 20 characters"),]
        public string FamilyName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password length must be greater than 6 characters"), MaxLength(12, ErrorMessage = "Password length must be less than 12 characters"),]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }// admin, evaluator
    }
}
