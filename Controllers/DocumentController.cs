using CMS_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CMS_Project.Controllers
{
    public class DocumentController : Controller
    {
        private readonly CMSContext _context;

        public DocumentController(CMSContext context)
        {
            _context = context;
        }

        // GET /Documents/

        [Authorize]
        public IActionResult Index()
        {
            var Documents = _context.Documents.ToList();
            return View(Documents);
        }

        // GET /Documents/5
        public IActionResult View(int id)
        {
            var Document = _context.Documents.Where(p=> p.Id==id);
            return View(Document);
        }

        // GET /Documents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST /Documents/Create [NOT DONE]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["message"] = "Obs something went wrong!";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET /Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Documents == null) 
            {
                return NotFound();
            }
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", document.UserId);
            ViewData["ContentType"] = new SelectList(_context.ContentTypes, "Id", "Id", document.ContentTypeId);
            ViewData["Folder"] = new SelectList(_context.Folders, "Id", "Id", document.FolderId);
            return View(document);
        }

        // GET /Documents/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.Include(p => p.Id).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }
        // POST /Documents/Delete
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Documents == null)
            {
                return Problem("Entity set 'context.Documents  is null.");
            }
            var document = _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Remove(document);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
