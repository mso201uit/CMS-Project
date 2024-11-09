using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using CMS_Project.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

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
            _userService = userService;
            _logger = logger;
            
        }

        // GET: api/Folder/all
        [HttpGet("all")]
        public async Task<IActionResult> GetFolders()
        {
            // Get NameIdentifier from claims to find user ID
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            
            // Mappe entitiene til DTO-er
            var folderDtos = await _folderService.GetAllFoldersAsDtoAsync(userId);
            
            // Hent brukerinformasjon fra tjenesten
            var user = await _userService.GetUserByIdAsync(userId);
            
            var response = new
            {
                User = new
                {
                    userId = user.Id,
                    user.Username,
                    user.Email,
                    user.CreatedDate
                },
                Folders = folderDtos
            };

            return Ok(response);
        }
        
        // Rekursiv mapping til FolderDto
        private FolderDto MapToFolderDto(Folder folder)
        {
            return new FolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                CreatedDate = folder.CreatedDate,
                ParentFolderId = folder.ParentFolderId,
                ChildrenFolders = folder.ChildrenFolders.Select(MapToFolderDto).ToList() ?? new List<FolderDto>()
            };
        }

        // GET: api/Folder/details/{id}
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

        // POST: api/Folder/create-folder
        [HttpPost("create-folder")]
        public async Task<IActionResult> CreateFolder([FromBody] FolderCreateDto  folderCreateDto)
        {
            //ModelState check
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Attempted to create a folder with invalid data.");
                return BadRequest(ModelState);
            }

            try
            {
                var userId = await _userService.GetUserIdFromClaimsAsync(User);
                _logger.LogInformation("Received CreateFolder request: {@folderCreateDto}", folderCreateDto);
                
                var folder = new Folder
                {
                    Name = folderCreateDto.Name,
                    ParentFolderId = folderCreateDto.ParentFolderId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };
                
                await _folderService.CreateFolderAsync(folder);
                _logger.LogInformation($"Folder created with ID {folder.Id}.");
                
                // Map folder til FolderDto for responsen
                var folderDto = MapToFolderDto(folder);

                return CreatedAtAction(nameof(GetFolder), new { id = folder.Id }, folderDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the folder.");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the folder.");
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
            // Get NameIdentifier from claims to find user ID
            var userId = await _userService.GetUserIdFromClaimsAsync(User);
            
            if (userId == -1)
            {
                return StatusCode(500, "UserId not found. User might not exist.");
            }

            try
            {
                var result = await _folderService.DeleteFolderAsync(id, userId);

                if (!result)
                {
                    _logger.LogWarning($"Folder with ID {id} was not found or does not belong to the user.");
                    return NotFound(new { message = $"Folder with ID {id} was not found or does not belong to the user." });
                }

                _logger.LogInformation($"Folder with ID {id} successfully deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting folder with ID {id}.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
