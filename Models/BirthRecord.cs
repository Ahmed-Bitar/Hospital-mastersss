using System.ComponentModel.DataAnnotations;
using MedicalPark.Models;

public enum NewbornCondition
{
    handiCapped = 1,
    GoodHealth,
    Sick,
    Steadysrrtate,
    Dead

}

public class BirthRecord:SurgicalOperations
{
    
    [Required]
    [Display(Name = "Birth Weight (grams)")]
    public int BirthWeight { get; set; }
    [Required]
    [Display(Name = "Birth Length (cm)")]
    public int BirthLength { get; set; }
    [Required]
    [Display(Name = "Newborn Condition")]
    public NewbornCondition NewbornCondition { get; set; }
   

    [Required]
    [Display(Name = "Father  Name")]
    public string FatherName { get; set; }

}
