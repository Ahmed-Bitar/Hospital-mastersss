namespace MedicalPark.Controllers
{
    public class BirthRecordDto
    {
        public int MotherId { get; set; }
        public int DoctorId { get; set; }
        public DateTime BirthDate { get; set; }
        public string Notes { get; set; }
    }
}
