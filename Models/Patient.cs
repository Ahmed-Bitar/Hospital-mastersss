using System.ComponentModel.DataAnnotations;

namespace MedicalPark.Models
{
    public enum MedicalCondition
    {

        [Display(Name = "Critical condition")]
        Criticalcondition = 0,
        [Display(Name = "Critical but stable condition")]
        Criticalbutstablecondition = 1,
        [Display(Name = "Steady state")]
        Steadystate = 2,
        [Display(Name = "Normal condition")]

        Normalcondition = 3,

    }

    public enum BloodType
    {

        [Display(Name = "A Positine")]
        Ap = 0,
        [Display(Name = "A Ngative")]
        An = 1,
        [Display(Name = "B Positine")]
        Bp = 2,
        [Display(Name = "B Ngative")]
        Bn = 3,
        [Display(Name = "BA Positine")]
        ABp = 4,
        [Display(Name = "BA Ngative")]
        ABn = 5,
        [Display(Name = "O Positine")]
        Op = 6,
        [Display(Name = "O Ngative")]
        On = 7


    }
    public class Patient : ApplicationUser
    {

        public int Age { get; set; }
        public string Adres { get; set; }

        public BloodType BloodType { get; set; }
        public string MedicalCondition { get; set; }


        public string DiseaseName { get; set; }
        public List<Doctor> Doctors { get; set; }
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
