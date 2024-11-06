using CMS_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMS_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly CMSContext _context;

        public DocumentController(CMSContext context)
        {
            _context = context;
        }

        // GET: api/Documents
        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            // Hent brukerens ID fra JWT-tokenet
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Hent alle dokumenter som tilhører brukeren
            var documents = await _context.Documents
                .Where(d => d.UserId == userId)
                .ToListAsync();
            return Ok(documents);
        }

        // GET: api/Documents/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDocument(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var Document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (Document == null)
            {
                return NotFound();
            }

            return Ok(Document);
        }

        // POST: api/Document
        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] Document document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            document.UserId = userId;
            document.CreatedDate = DateTime.Now;

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
        }

        // PUT api/Documents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] Document document)
        {
            if (id != document.Id)
            {
                return BadRequest("ID i URL stemmer ikke overens med ID i dataen.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (document.UserId != userId)
            {
                return Unauthorized();
            }

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id, userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (document == null)
            {
                return NotFound();
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentExists(int id, int userId)
        {
            return _context.Documents.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}
