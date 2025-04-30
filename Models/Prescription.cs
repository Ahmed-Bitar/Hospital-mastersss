namespace MedicalPark.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Sickness { get; set; }
        public string MedicalsName { get; set; }
        public DateTime CreatedDate { get; set; }

        public int AppointmentId { get; set; }
        public int DoctorID { get; set; }

        public Appointment Appointment { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
        public int PatientID { get; set; }
    }
}
