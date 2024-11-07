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
        /// <returns>List of folders by given user id</returns>
        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(int UserId)
        {
            return await _context.Folders
                .Include(f => f.Documents)
                .Include(f => f.User)
                .Where(f => f.UserId == UserId)
                .ToListAsync();
        }

        /// <summary>
        /// GET folder by id given
        /// </summary>
        /// <param name="id"></param>
        /// <returns>folder by id given</returns>
        public async Task<Folder> GetFolderByIdAsync(int id)
        {
            return await _context.Folders
                .Include(f => f.Documents)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// CREATE folder by Dto and checks ownership
        /// </summary>
        /// <param name="folderDto"></param>
        /// <param name="userId"></param>
        /// <returns>document created</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Folder> CreateFolderAsync(FolderDto folderDto, int userId)
        {
            var folder = new Folder
            {
                Name = folderDto.Name,
                UserId = userId,
                ParentFolderId = folderDto.ParentFolderId,
                CreatedDate = DateTime.UtcNow
            };

            //Check if user owns parent folder:
            if (folder.ParentFolderId != null)
            {
                var parentfolder = await _context.Folders.FirstOrDefaultAsync(f => f.Id == folderDto.ParentFolderId);
                if(parentfolder != null)
                {
                    if (folder.UserId != parentfolder.UserId)
                        throw new ArgumentException("User doesn't own parent folder.");
                }
            }

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

            return folder;
        }

        /// <summary>
        /// UPDATE folder by given id and checks ownership. Updates by dto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateFolderDto"></param>
        /// <param name="userId"></param>
        /// <returns>true if completed, false if not</returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// DELETE folder by given id and checks ownership
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>true when deleted and false if failed</returns>
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

        /// <summary>
        /// checks if folder with id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true if exists, and false if not</returns>
        private async Task<bool> FolderExists(int id)
        {
            return await _context.Folders.AnyAsync(f => f.Id == id);
        }
    }
}
