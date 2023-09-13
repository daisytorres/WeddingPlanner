#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace WeddingPlanner.Models;

public class Wedding 
{
    [Key]  
    public int WeddingId { get; set; }


    [Required]
    public string WedderOne { get; set; }

    [Required]
    public string WedderTwo { get; set; }

    [Required]
    [FutureDate]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public string Address { get; set; }

    public DateTime CreatedAt {get;set;} = DateTime.Now;   
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    //FK
    public int UserId { get; set; }
    //Nav Prop
    public User? Planner { get; set; }
    public List <UserRSVP> UsersGoing { get; set;} = new();

}



public class FutureDateAttribute: ValidationAttribute
{
    protected override ValidationResult IsValid (object value, ValidationContext validationContext)
    {
        if (((DateTime)value) < DateTime.Now)
        {
            return new ValidationResult ("Date must be in the future");
        } else {
            return ValidationResult.Success;
        }
    }
}