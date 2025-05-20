using System.ComponentModel.DataAnnotations;
using MedicalPark.Models;

public class Invoice
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    [Required]
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

}
