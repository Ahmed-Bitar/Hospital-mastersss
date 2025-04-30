namespace MedicalPark.Models
{
   
    public class Doctor: ApplicationUser
    {

        public enum DoctorSpecialty
        {
            GeneralPractitioner,
            Cardiologist,
            Dermatologist,
            Neurologist,
            Orthopedic,
            Pediatrician,
            Gynecologist,
            Psychiatrist,
            Surgeon,
            Endocrinologist,
            Urologist,
            ENTDoctor,
            Ophthalmologist,
            Rheumatologist,
            Pathologist,
            Oncologist,
            Anesthesiologist,
            Radiologist,
            FamilyMedicine,
            InternalMedicine,
            Geriatrics
        }
        public DoctorSpecialty Specialty { get; set; }
        public int Salery { get; set; }
        public int PatientID { get; set; }

        public List<Patient> Patients { get; set; } = [];
        public List<Appointment> Appointments { get; set; } = [];
        public List<MedicalRecord> MedicalRecords { get; set; } = [];
        public List<Prescription> Prescriptions { get; set; } = [];


    }
}