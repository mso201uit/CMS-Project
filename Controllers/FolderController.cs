using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using CMS_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;


namespace CMS_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;
        private readonly ILogger<FolderController> _logger;
        private readonly IUserService _userService;


        public FolderController(IFolderService folderService, ILogger<FolderController> logger, IUserService userService)
        {
            _folderService = folderService;
            _logger = logger;
            _userService = userService;
            
        }

        // GET: api/Folder
        [HttpGet]
        public async Task<IActionResult> GetFolders()
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
                var folders = await _folderService.GetAllFoldersAsync(userId);
                return Ok(folders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching folders.");
                return StatusCode(500, "En uventet feil oppstod.");
            }
        }

        // GET: api/Folder/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolder(int id)
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
                var folder = await _folderService.GetFolderByIdAsync(id);
                if (folder == null)
                {
                    _logger.LogWarning($"Folder with ID {id} was not found.");
                    return NotFound(new { message = $"Folder with ID {id} was not found." });
                }
                if (folder.UserId == userId)
                {
                    return Ok(folder);
                }

                return StatusCode(500, "User doesn't own this folder.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Folder with ID {id} was not found.");
                return NotFound(new { message = $"Folder with ID  {id} was not found." });
            }
        }

        // POST: api/Folder
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] FolderDto folderDto)
        {
            //ModelState check
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Attempted to create a folder with invalid data.");
                return BadRequest(ModelState);
            }

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
                var createdFolder = await _folderService.CreateFolderAsync(folderDto, userId);
                _logger.LogInformation($"Folder created with ID {createdFolder.Id}.");

                return CreatedAtAction(nameof(GetFolder), new { id = createdFolder.Id }, createdFolder);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while creating: {folderDto.Name}");
                return StatusCode(500, "Unexpected error occured.");
            }
        }

        // PUT: api/Folder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFolder(int id, [FromBody] UpdateFolderDto updateFolderDto)
        {
            //ModelState check
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Attempted to update folder with ID {id} with invalid data.");
                return BadRequest(ModelState);
            }
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
                var result = await _folderService.UpdateFolderAsync(id, updateFolderDto, userId);
                if (!result)
                {
                    _logger.LogWarning($"Folder with ID {id} was not found for update.");
                    return NotFound(new { message = $"Mappe med ID {id} ble ikke funnet." });
                }

                _logger.LogInformation($"Folder with ID {id} was updated successfully.");
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating folder with ID {id}.");
                return StatusCode(500, "An unexpected Error occured.");
            }
        }

        // DELETE: api/Folder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(int id)
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
                var result = await _folderService.DeleteFolderAsync(id, userId);
                if (!result)
                {
                    _logger.LogWarning($"Folder with ID {id} was not found for deletion.");
                    return NotFound(new { message = $"Folder with ID {id} was not found." });
                }

                _logger.LogWarning($"Folder with ID {id} was not found for deletion.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting folder with ID {id}.");
                return StatusCode(500, "An unexpected error occured.");
            }
        }
    }
}
