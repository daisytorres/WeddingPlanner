using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WeddingPlanner.Controllers;


[SessionCheck]
public class WeddingController : Controller //Update names here for whatever controller you are in
{
    private readonly ILogger<WeddingController> _logger;
    private MyContext _context; //DO NOT FORGET THIS 

    public WeddingController(ILogger<WeddingController> logger, MyContext context) //DO NOT FORGET TO ADD AFTER THE , 
    {
        _logger = logger;
        _context = context; //DO NOT FORGET TO ADD THIS
    }



//Route that will render the dashboard
    [HttpGet("weddings")]
    public IActionResult Dashboard()
    {
        List<Wedding> allWeddings = _context.Weddings.Include(w => w.UsersGoing).ToList();
        return View(allWeddings);
    }



//route that will render the view (form to submit)
    [HttpGet("weddings/new")]
    public ViewResult NewWedding()
    {
        return View();
    }



//Route that will post the form into our db
    [HttpPost("weddings/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if (!ModelState.IsValid)
        {
            return View("NewWedding");
        }
        newWedding.UserId = (int)HttpContext.Session.GetInt32("UniqueUserID");
        _context.Add(newWedding);
        _context.SaveChanges();
        // return RedirectToAction("Dashboard");
        return ViewWedding(newWedding.WeddingId);

    }



//route that will be for deleting a wedding
    [HttpPost("weddings/{id}/delete")]
    public RedirectToActionResult DeleteWedding(int id)
    {
        Wedding? toDelete = _context.Weddings.SingleOrDefault(w => w.WeddingId == id);
        if (toDelete != null)
        {
            _context.Remove(toDelete);
            _context.SaveChanges();
        }
        return RedirectToAction("Dashboard");
    }



//route for toggle (RSVP)
    [HttpPost("weddings/{id}/rsvp")]
    public RedirectToActionResult ToggleRSVP(int id)
    {
        int UniqueUserID = (int)HttpContext.Session.GetInt32("UniqueUserID");
        UserRSVP existingRSVP = _context.UserRSVPs.FirstOrDefault(r => r.WeddingID == id && r.UserId == UniqueUserID);
        if (existingRSVP == null) //if it does not already exists, we will create it
        {
            UserRSVP newRSVP = new()
            {
                UserId = UniqueUserID,
                WeddingID = id
            };
            _context.Add(newRSVP);
        }
        else //if we did find it, meaning they already RSVPd
        {
            _context.Remove(existingRSVP);
        }
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }



//route for view one wedding
    [HttpGet("weddings/{id}/view")]
    public IActionResult ViewWedding(int id)
    {
        Wedding? oneWedding = _context.Weddings
                            .Include(w => w.UsersGoing)
                            .ThenInclude(rs => rs.UserConfirmed)
                            .FirstOrDefault(w => w.WeddingId == id);
        if (oneWedding == null)
        {
            return RedirectToAction("Dashboard");
        }
        return View("ViewWedding", oneWedding);
    }


//route for editing wedding
    [HttpGet("weddings/{id}/edit")]
    public IActionResult EditWedding(int id)
    {
        Wedding? ToBeEdited = _context.Weddings.FirstOrDefault(w => w.WeddingId == id); 
        if (ToBeEdited == null)
        {
            return RedirectToAction("Dashboard"); //if they manually edited URL or something we we got a null item
        }
        return View(ToBeEdited); //passing this in so that it can be our editing model in the viewing page
    }



//route to process view edit changes
    [HttpPost("weddings/{id}/update")]
    public IActionResult UpdateWedding(int id, Wedding editedWedding) 
    {
        Wedding? ToBeUpdated = _context.Weddings.FirstOrDefault(w => w.WeddingId == id);
        if(!ModelState.IsValid || ToBeUpdated == null) 
        {
            return View("EditWedding", ToBeUpdated); //pass in the model/object (can do ToBeUdpated + add value tags in edit form for each one, OR editDish which keeps whatever changes they made )
        } //otherwise updated all keys
        ToBeUpdated.WedderOne = editedWedding.WedderOne;
        ToBeUpdated.WedderTwo = editedWedding.WedderTwo;
        ToBeUpdated.Date = editedWedding.Date;
        ToBeUpdated.Address = editedWedding.Address;
        ToBeUpdated.UpdatedAt = DateTime.Now;

        _context.SaveChanges();

        return RedirectToAction("ViewWedding", new {id=id});
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}