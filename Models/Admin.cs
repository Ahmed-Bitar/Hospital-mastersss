using System.ComponentModel.DataAnnotations;

namespace MedicalPark.Models
{
    public enum Department
    {
        [Display(Name = "Cardiology")]
        Cardiology = 0,

        [Display(Name = "Neurology")]
        Neurology = 1,

        [Display(Name = "Pediatrics")]
        Pediatrics = 2,

        [Display(Name = "Radiology")]
        Radiology = 3,

        [Display(Name = "Oncology")]
        Oncology = 4,

        [Display(Name = "Orthopedics")]
        Orthopedics = 5,

        [Display(Name = "Emergency")]
        Emergency = 6,

        [Display(Name = "Administration")]
        Administration = 7,

        [Display(Name = "Pharmacy")]
        Pharmacy = 8,

        [Display(Name = "Laboratory")]
        Laboratory = 9,

        [Display(Name = "SurgeryOption")]
        Surgery = 10
    }
    public enum ManegmentType
    {

        [Display(Name = "Hospital Director")]
        Director = 0,
        [Display(Name = "Hospital Secretary")]
        Secretary = 1,

        [Display(Name = "Finance Manager ")]
        Finance = 2,


    }


    public class Admin:ApplicationUser
    {
        public ManegmentType Type { get; set; }
        public Department Department { get; set; }
    }
}
