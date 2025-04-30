namespace MedicalPark.Models
{
    public class Appointment
    {

        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime Rendezvous { get; set; }
        public string Sickness { get; set; }
        public int DoctorId { get; set; }


        public Doctor Doctor { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public Prescription Prescription { get; set; }

    }
}
