using System.ComponentModel.DataAnnotations;

namespace MedicalPark.Models
{
    public enum HospitalRoomType
    {
        PatientRoom,        
        OperatingRoom,      
        EmergencyRoom,      
        IntensiveCareUnit,  
        ExaminationRoom,    
        DeliveryRoom,       
        SterilizationRoom,  
        WaitingRoom,        
        ConsultationRoom,   
        RecoveryRoom        
    }
    public enum PatientRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum OperatingRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum EmergencyRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum IntensiveCareUnit
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum ExaminationRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum DeliveryRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum SterilizationRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum WaitingRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum ConsultationRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public enum RecoveryRoom
    {
        Section1 = 1, Section2, Section3, Section4, Section5,
        Section6, Section7, Section8, Section9, Section10,
        Section11, Section12, Section13, Section14, Section15,
        Section16, Section17, Section18, Section19, Section20
    }

    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string RoomName  { get; set; }

        [Required]

        public HospitalRoomType RoomType { get; set; }
        [Required]
        public PatientRoom PatientRoomSection { get; set; }
        [Required]
        public OperatingRoom OperatingRoomSection { get; set; }


        [Required]

        public EmergencyRoom EmergencyRoomSection { get; set; }
        [Required]

        public IntensiveCareUnit IntensiveCareUnitSection { get; set; }
        [Required]

        public ExaminationRoom ExaminationRoomSection { get; set; }
        [Required]

        public DeliveryRoom DeliveryRoomSection { get; set; }
        [Required]

        public SterilizationRoom SterilizationRoomSection { get; set; }
        [Required]

        public WaitingRoom WaitingRoomSection { get; set; }
        [Required]

        public ConsultationRoom ConsultationRoomSection { get; set; }
        [Required]

        public RecoveryRoom RecoveryRoomSection { get; set; }
        
        [Required]
       public bool IsAvailable { get; set; } = false;


        public bool IsSterile { get; set; } = false;
        public bool IsDeleted { get; internal set; }
    }
}
