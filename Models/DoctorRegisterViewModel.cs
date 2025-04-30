using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static MedicalPark.Models.Doctor;

namespace MedicalPark.Models
{
    public class DoctorRegisterViewModel 
    {
  

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DoctorSpecialty Specialty { get; set; }

        [Required]
        public int Salary { get; set; }

        [Display(Name = "User Type (Role)")]
        public required string UserType { get; set; } = "Doctor";


        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public int PatientId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
