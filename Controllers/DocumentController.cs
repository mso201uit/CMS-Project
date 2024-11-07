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
        private readonly IUserService _userService;

        public DocumentController(IDocumentService documentService, IUserService userService)
        {
            _documentService = documentService;
            _userService = userService;
        }

        // GET: api/Documents
        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            //Get NameIdentifier from claims from user to get username, which then service gets userId to find folder user owns.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = await _userService.GetUserIdAsync(claims.Value);
            if (userId == -1)
            {
                return StatusCode(500, "UserId not found. User might not exist.");
            }

            try
            {
                var documents = await _documentService.GetAllDocumentsAsync(userId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Documents/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDocument(int id)
        {
            //Get NameIdentifier from claims from user to get username, which then service gets userId to find folder user owns.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = await _userService.GetUserIdAsync(claims.Value);
            if (userId == -1)
            {
                return StatusCode(500, "UserId not found. User might not exist.");
            }
            try
            {
                //get document according to id
                var document = await _documentService.GetDocumentByIdAsync(id);
                //check if the user owns the document (could put this check inside the service)
                if (document.UserId == userId)
                {
                    return Ok(document);
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Document
        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] DocumentDto documentDto)
        {
            //ModelState check
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Get NameIdentifier from claims from user to get username, which then service gets userId to find folder user owns.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = await _userService.GetUserIdAsync(claims.Value);
            if (userId == -1)
            {
                return StatusCode(500, "UserId not found. User might not exist.");
            }

            try
            {
                var createdDocument = await _documentService.CreateDocumentAsync(documentDto, userId);
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
            //ModelState check
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Get NameIdentifier from claims from user to get username, which then service gets userId to find folder user owns.
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
                    return NotFound();

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
            catch (Exception)
            {
                return StatusCode(500, "Unexpected error occured.");
            }
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            //Get NameIdentifier from claims from user to get username, which then service gets userId to find folder user owns.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = await _userService.GetUserIdAsync(claims.Value);
            if (userId == -1)
            {
                return StatusCode(500, "UserId not found. User might not exist.");
            }

            try
            {
                var result = await _documentService.DeleteDocumentAsync(id, userId);
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
