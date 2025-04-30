using System.ComponentModel.DataAnnotations;

namespace MedicalPark.Models
{
    public class LoginModel
    {
        [Required]
        public int Id { get; set; } 
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
