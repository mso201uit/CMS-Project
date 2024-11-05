using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CMS_Project.Data;
using CMS_Project.Models;

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
    
    // Add a new user
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    public IActionResult Delete(User id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var user = _context.Users.Find(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
    
}