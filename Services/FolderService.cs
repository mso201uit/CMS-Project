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

        /// <summary>
        /// GET all folders where UserId = Documents-User.Id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
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
                ParentFolderId = folderDto.ParentFolderId,
                CreatedDate = DateTime.UtcNow
            };

            //Check if user owns parent folder:
            if (folder.ParentFolderId != null)
            {
                var parentfolder = await _context.Folders.FirstOrDefaultAsync(f => f.Id == folderDto.ParentFolderId);
                if(parentfolder != null)
                {
                    if (folderDto.UserId != parentfolder.UserId)
                        throw new ArgumentException("User doesn't own parent folder.");
                }
            }

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

            return folder;
        }

        public async Task<bool> UpdateFolderAsync(int id, UpdateFolderDto updateFolderDto, int userId)
        {
            //check if folder exists
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
                throw new ArgumentException("folder not found.");
            //check if user owns folder.
            if (folder.UserId != userId)
                throw new ArgumentException("User doesn't own folder.");

            //if parentfolder exists:
            if (updateFolderDto.ParentFolderId != null)
            {
                //check if user owns parent folder.
                var parentfolder = await _context.Folders.FirstAsync(f => f.Id == updateFolderDto.ParentFolderId);
                if (folder.ParentFolderId == null)
                    if (parentfolder.UserId != userId)
                        throw new ArgumentException("User doesn't own parent folder.");
            }

            folder.Name = updateFolderDto.Name;
            folder.ParentFolderId = updateFolderDto.ParentFolderId;

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

        public async Task<bool> DeleteFolderAsync(int id, int userId)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
                return false;

            // Sletter alle dokumenter i mappen eller håndter relasjonen på annen måte
            // Eksempel: Hvis du har satt opp Cascade Delete, vil tilknyttede dokumenter slettes automatisk
            if (userId == folder.UserId)
            {
                _context.Folders.Remove(folder);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        private async Task<bool> FolderExists(int id)
        {
            return await _context.Folders.AnyAsync(f => f.Id == id);
        }
    }
}
