using CMS_Project.Models.DTOs;
using CMS_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
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


        public FolderController(IFolderService folderService, ILogger<FolderController> logger)
        {
            _folderService = folderService;
            _logger = logger;

            
        }

        // GET: api/Folder
        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {
            try
            {
                var folders = await _folderService.GetAllFoldersAsync();
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
            try
            {
                var folder = await _folderService.GetFolderByIdAsync(id);
                if (folder == null)
                {
                    _logger.LogWarning($"Folder with ID {id} was not found.");
                    return NotFound(new { message = $"Mappe med ID {id} ble ikke funnet." });
                }

                return Ok(folder);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Folder with ID {id} was not found.");
                return NotFound(new { message = $"Mappe med ID {id} ble ikke funnet." });
            }
        }

        // POST: api/Folder
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] FolderDto folderDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Attempted to create a folder with invalid data.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdFolder = await _folderService.CreateFolderAsync(folderDto);
                _logger.LogInformation($"Folder created with ID {createdFolder.Id}.");

                return CreatedAtAction(nameof(GetFolder), new { id = createdFolder.Id }, createdFolder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"En feil oppstod under opprettelse av mappe: {folderDto.Name}");
                return StatusCode(500, "En uventet feil oppstod.");
            }
        }

        // PUT: api/Folder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFolder(int id, [FromBody] UpdateFolderDto updateFolderDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Attempted to update folder with ID {id} with invalid data.");
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _folderService.UpdateFolderAsync(id, updateFolderDto);
                if (!result)
                {
                    _logger.LogWarning($"Folder with ID {id} was not found for update.");
                    return NotFound(new { message = $"Mappe med ID {id} ble ikke funnet." });
                }

                _logger.LogInformation($"Folder with ID {id} was updated successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating folder with ID {id}.");
                return StatusCode(500, "En uventet feil oppstod.");
            }
        }

        // DELETE: api/Folder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            try
            {
                var result = await _folderService.DeleteFolderAsync(id);
                if (!result)
                {
                    _logger.LogWarning($"Folder with ID {id} was not found for deletion.");
                    return NotFound(new { message = $"Mappe med ID {id} ble ikke funnet." });
                }

                _logger.LogWarning($"Folder with ID {id} was not found for deletion.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting folder with ID {id}.");
                return StatusCode(500, "En uventet feil oppstod.");
            }
        }
    }
}
