#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;

namespace WeddingPlanner.Models;

public class UserRSVP 
{
    [Key]  
    public int UserRSVPId { get; set; }

    public int UserId { get; set; }
    public User? UserConfirmed { get; set; }

    public int WeddingID { get; set; }
    public Wedding? WeddingConfirmed { get; set; }

    public DateTime CreatedAt {get;set;} = DateTime.Now;   
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}