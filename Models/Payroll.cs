using System.ComponentModel.DataAnnotations;
using MedicalPark.Models;

public class Payroll
{
    public int Id { get; set; }

    [Required]
    public int StaffId { get; set; }
    public ApplicationUser Staff { get; set; } 

    public decimal Salary { get; set; }

    public int WorkingHours { get; set; }

    public decimal Bonuses { get; set; }

    public decimal Deductions { get; set; }

    public DateTime PayrollDate { get; set; } = DateTime.Now;
}
