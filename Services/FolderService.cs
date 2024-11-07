using CMS_Project.Data;
using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CMS_Project.Services
{
    public class FolderService : IFolderService
    {
        private readonly CMSContext _context;

        public FolderService(CMSContext context)
        {
            _context = context;
        }

        //GET all folders where UserId = Documents-User.Id
        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(int UserId)
        {
            return await _context.Folders
                .Include(f => f.Documents)
                .Include(f => f.User)
                .Where(f => f.UserId == UserId)
                .ToListAsync();
        }

        public async Task<Folder> GetFolderByIdAsync(int id)
        {
            return await _context.Folders
                .Include(f => f.Documents)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Folder> CreateFolderAsync(FolderDto folderDto)
        {
            var folder = new Folder
            {
                Name = folderDto.Name,
                UserId = folderDto.UserId,
                CreatedDate = DateTime.UtcNow
            };

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

            return folder;
        }

        public async Task<bool> UpdateFolderAsync(int id, UpdateFolderDto updateFolderDto)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
                return false;

            folder.Name = updateFolderDto.Name;

            _context.Entry(folder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FolderExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> DeleteFolderAsync(int id)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
                return false;

            // Sletter alle dokumenter i mappen eller håndter relasjonen på annen måte
            // Eksempel: Hvis du har satt opp Cascade Delete, vil tilknyttede dokumenter slettes automatisk
            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> FolderExists(int id)
        {
            return await _context.Folders.AnyAsync(f => f.Id == id);
        }
    }
}
