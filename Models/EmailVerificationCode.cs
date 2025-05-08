namespace MedicalPark.Models
{
    public class EmailVerificationCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }      
        public DateTime SentAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

}
