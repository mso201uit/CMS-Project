using CMS_Project.Models.DTOs;
using CMS_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMS_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IUserService _userService;
        private readonly ILogger<DocumentController> _logger;


        public DocumentController(IDocumentService documentService, IUserService userService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _userService = userService;
            _logger = logger;
        }
        
        // POST: api/Document/create-document
        [HttpPost("create-document")]
        public async Task<IActionResult> CreateDocument([FromBody] DocumentCreateDto documentCreateDto)
        {
            //ModelState check
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Attempted to create a document with invalid data.");
                return BadRequest(ModelState);
            }

            try
            {
                var userId = await _userService.GetUserIdFromClaimsAsync(User);
                var createdDocument = await _documentService.CreateDocumentAsync(documentCreateDto, userId);
                
                return CreatedAtAction(nameof(GetDocumentById), new { id = createdDocument.Document.DocumentId }, createdDocument);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the document.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // GET: api/Document/all
        [HttpGet("all")]
        public async Task<IActionResult> GetDocuments()
        {
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            var documents = await _documentService.GetAllDocumentsAsync(userId);
            return Ok(documents);
        }

        // GET: api/Documents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDocumentById(int id)
        {
            try
            {
                var userId = await _userService.GetUserIdFromClaimsAsync(User);
                var document = await _documentService.GetDocumentByIdAsync(id, userId);
                return Ok(document);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving document with ID {id}.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // PUT: api/Documents/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentDto updateDocumentDto)
        {
            //ModelState check
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Attempted to update document with ID {id} with invalid data.");
                return BadRequest(ModelState);

            }
            
            // Get userId from claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = await _userService.GetUserIdAsync(claims.Value);
            if (userId == -1)
            { 
                return StatusCode(500, "UserId not found. User might not exist.");
            }

            try
            { 
                var result = await _documentService.UpdateDocumentAsync(id, updateDocumentDto, userId);
                if (!result)
                    {
                        _logger.LogWarning($"Document with ID {id} was not found for update."); 
                        return NotFound(new { message = $"Document with ID {id} was not found." });
                    }
                    _logger.LogInformation($"Document with ID {id} was updated successfully.");
                    return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            { 
                return Unauthorized(new { message = ex.Message });
            }
            catch (DbUpdateConcurrencyException)
            { 
                return StatusCode(500, "An error occursed when updating the file.");
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, $"An unexpected error occurred while updating document with ID {id}.");
                return StatusCode(500, "Unexpected error occured.");
            }
        }

        // DELETE: api/Documents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            // Get user ID from claims
            var userId = await _userService.GetUserIdFromClaimsAsync(User);

            if (userId == -1)
            {
                return StatusCode(500, "UserId not found. User might not exist.");
            }

            try
            {
                var result = await _documentService.DeleteDocumentAsync(id, userId);

                if (!result)
                {
                    _logger.LogWarning($"Document with ID {id} was not found or does not belong to the user.");
                    return NotFound(new
                        { message = $"Document with ID {id} was not found or does not belong to the user." });
                }

                _logger.LogInformation($"Document with ID {id} successfully deleted.");
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting document with ID {id}.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
