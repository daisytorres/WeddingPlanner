using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace WeddingPlanner.Controllers;



public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private MyContext _context; //DO NOT FORGET THIS 

    public UserController(ILogger<UserController> logger, MyContext context) //DO NOT FORGET TO ADD AFTER THE , 
    {
        _logger = logger;
        _context = context; //DO NOT FORGET TO ADD THIS
    }



//route to display the forms for register and login
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }



//route that will display once a User is logged in
    // [HttpGet("users/success")]
    // public IActionResult Success()
    // {
    //     return View("Dashboard");
    // }



//route for submitting information/creating account
    [HttpPost("users/register")]
    public IActionResult RegisterUser (User newUser)
    {
        if (!ModelState.IsValid) //checking if validations pass
        {
            return View("Index"); //if do not pass, render same form/view
        }
        PasswordHasher<User> hasher = new(); //built in function that will hash passwords, Indicate the object to hash and then create as variable
        newUser.Password = hasher.HashPassword(newUser, newUser.Password);
//taking in newUser's password and setting it = to hasher.Hashpassword and it takes in the new object and it's updated key as the parameters
    _context.Add(newUser); //now we can add user to DB and save
    _context.SaveChanges();

    //If able to sucessfully create and add new user, send them to desired page and use session to ensure it's their unique page
    HttpContext.Session.SetInt32("UniqueUserID", newUser.UserId); //using int since user ID is an int
    HttpContext.Session.SetString("LoggedUserName", newUser.FirstName); //since we need their name on every page
    return RedirectToAction("Dashboard", "Wedding"); //view page, controller
    }



//*****************************************************************************************************************************************************//


//route for login user 
    [HttpPost("users/login")]
    public IActionResult LoginUser (LogUser logAttempt)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        User? dbUser = _context.Users.FirstOrDefault(u => u.Email == logAttempt.LogEmail);
        if (dbUser == null ) //did not find user
        {
            ModelState.AddModelError("LogPassword", "Invalid credentials");
            return View ("Index");
        }
        PasswordHasher<LogUser> hasher = new();
        PasswordVerificationResult pwCompareResult = hasher.VerifyHashedPassword(logAttempt,dbUser.Password,logAttempt.LogPassword);
        if (pwCompareResult == 0)
        {
            ModelState.AddModelError("LogPassword", "Invalid credentials");
            return View ("Index");
        }
        HttpContext.Session.SetInt32("UniqueUserID", dbUser.UserId); 
        HttpContext.Session.SetString("LoggedUserName", dbUser.FirstName);

        return RedirectToAction("Dashboard", "Wedding");
    }



//*****************************************************************************************************************************************************//


//Route for logout
    [HttpPost("users/logout")]
    public IActionResult LogOut()
    {
        // HttpContext.Session.Clear(); //this will delete everything though, so update to what we actually want to clear
        HttpContext.Session.Remove("UniqueUserID"); //only removing the unique user id key
        return RedirectToAction("Index");
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}