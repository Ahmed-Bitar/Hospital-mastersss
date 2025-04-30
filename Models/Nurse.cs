namespace MedicalPark.Models
{


    public class Nurse : ApplicationUser
    {

        public int Salary { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }



    }
}