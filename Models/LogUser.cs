#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;


public class LogUser
{
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email")] //so that it is not called LogEmail on display, update display name
    public string LogEmail { get; set; } //since email and password are same page, give diff names for error validation to know which one
    
    [Required]
    [DataType(DataType.Password)] //makes it display as a password on the form
    [Display(Name = "Password")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string LogPassword { get; set; } 
    
}