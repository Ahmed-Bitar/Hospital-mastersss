using MedicalPark.Models;
using System.ComponentModel.DataAnnotations;

public class PatientConditionAfterSurgery
{
    public enum PatientStatus
    {
        Stable,          
        Critical,        
        InIntensiveCare,
        Deceased,        
        UnderObservation,
        Recovering,      
        Discharged       
    }

    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int SurgicalOperationId { get; set; }  

    [Required]
    public DateTime Date { get; set; } = DateTime.Now;

    [Required]
    public PatientStatus Status { get; set; }

    [MaxLength(500)]
    public string Notes { get; set; }

    public Patient Patient { get; set; }

    public SurgicalOperation SurgicalOperation { get; set; }  
}
