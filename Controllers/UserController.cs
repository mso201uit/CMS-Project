using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CMS_Project.Data;

namespace CMS_Project.Controllers;

public class UserController : Controller
{
    private readonly CMSContext _context;

    public UserController(CMSContext context)
    {
        _context = context;
    }
    
    // GET /User/
    public IActionResult Index()
    {
        var users = _context.Users.ToList();
        return View(users);
    }
    // Leg til mer
    
}