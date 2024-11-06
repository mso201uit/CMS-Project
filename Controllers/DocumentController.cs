using CMS_Project.Models.DTOs;
using CMS_Project.Models;
using CMS_Project.Services;
using CMS_Project.Data;
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
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // GET: api/Documents
        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }

        // GET: api/Documents/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDocument(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound();

            return Ok(document);
        }

        // POST: api/Document
        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] DocumentDto documentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdDocument = await _documentService.CreateDocumentAsync(documentDto);
                return CreatedAtAction(nameof(GetDocument), new { id = createdDocument.Id }, createdDocument);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "En uventet feil oppstod.");
            }
        }

        // PUT api/Documents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentDto  updateDocumentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Hent brukerens ID fra claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Bruker er ikke autentisert.");
            }

            try
            {
                var result = await _documentService.UpdateDocumentAsync(id, updateDocumentDto, userId);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "En feil oppstod under oppdatering av dokumentet.");
            }
            catch (Exception)
            {
                return StatusCode(500, "En uventet feil oppstod.");
            }
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            // Hent brukerens ID fra claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Bruker er ikke autentisert.");
            }

            try
            {
                var result = await _documentService.DeleteDocumentAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "En uventet feil oppstod.");
            }
        }
    }
}
