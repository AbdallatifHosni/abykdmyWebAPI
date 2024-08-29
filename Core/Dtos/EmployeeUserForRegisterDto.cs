using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Dtos
{
    public class EmployeeUserForRegisterDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "FirstName length must be greater than 2 characters"), MaxLength(20, ErrorMessage = "First name length must be less than 20 characters"),]
        public string FirstName { get; set; }
        public int IsActive { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "FamilyName length must be greater than 2 characters"), MaxLength(20, ErrorMessage = "Family name length must be less than 20 characters"),]
        public string FamilyName { get; set; }
     
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public int AutogeneratePassword { get; set; } = 1;
        public int SendPasswordByEmail { get; set; } = 1;
        public string PhotoUrl { get; set; }
        public string Password { get; set; }
        public List<int> GrantedServiceList { get; set; } = new List<int>();
        [Required]
        public string Role { get; set; }// admin, requester,employee
    }
}
