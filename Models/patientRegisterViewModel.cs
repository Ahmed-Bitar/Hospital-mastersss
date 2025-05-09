using System.ComponentModel.DataAnnotations;

namespace MedicalPark.Models
{
    public class patientRegisterViewModel
    {
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
       
        [Display(Name = "Full Name")] 
        public string Name { get; set; }
   

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "SelectedRoles")]
        public string UserType { get; set; } = "Patient";

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "Age")]
        public int Age { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Adres { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
