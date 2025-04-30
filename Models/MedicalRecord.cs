namespace MedicalPark.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string PationName { get; set; }
        public string Medicals { get; set; }
        public int PatientId { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }
}
