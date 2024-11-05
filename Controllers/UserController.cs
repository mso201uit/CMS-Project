using Microsoft.AspNetCore.Mvc;

namespace CMS_Project.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}