namespace MedicalPark.Models
{


    public class Nurse : ApplicationUser
    {

        public int Salary { get; set; }
        public bool IsDeleted { get; set; }



    }
}