using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CMS_Project.Data;

namespace CMS_Project.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}   