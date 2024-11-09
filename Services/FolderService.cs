using System.Collections.Generic;
using System.Threading.Tasks;
using CMS_Project.Data;
using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CMS_Project.Services
{
    public class FolderService : IFolderService
    {
        private readonly CMSContext _context;
        private IFolderService _folderServiceImplementation;

        public FolderService(CMSContext context)
        {
            _context = context;
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
                .Include(f => f.ChildrenFolders)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Gets all folders for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose folders are to be retrieved.</param>
        /// <returns>A list of folders belonging to the specified user.</returns>
        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(int userId)
        {
            return await _context.Folders
                .Include(f => f.Documents)
                .Include(f => f.User)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Retrieves all top-level folders (those without a parent folder) for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose top-level folders are to be retrieved.</param>
        /// <returns>A list of top-level folders belonging to the specified user, including their child folders.</returns>
        public async Task<IEnumerable<Folder>> GetFoldersByUserIdAsync(int userId)
        {
            return await _context.Folders
                .Where(f => f.UserId == userId && f.ParentFolderId == null)
                .Include(f => f.ChildrenFolders)
                .ToListAsync();
        }
        
        /// <summary>
        /// GET all folders where UserId = Documents-User.Id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>List of folders by given user id</returns>
        public async Task<List<FolderDto>> GetAllFoldersAsDtoAsync(int userId)
        {
            var rootFolders = await _context.Folders
                .Where(f => f.UserId == userId && f.ParentFolderId == null)
                .Include(f => f.ChildrenFolders)
                .ToListAsync();

            var folderDtos = rootFolders.Select(MapToFolderDto).ToList();
            return folderDtos;
        }
        
        private FolderDto MapToFolderDto(Folder folder)
        {
            return new FolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                CreatedDate = folder.CreatedDate,
                ParentFolderId = folder.ParentFolderId,
                ChildrenFolders = folder.ChildrenFolders?.Select(MapToFolderDto).ToList() ?? new List<FolderDto>()
            };
        }
        
        /// <summary>
        /// CREATE folder by Dto and checks ownership. First folder needs parentFolderId to be null!!
        /// </summary>
        /// <param name="folderDto"></param>
        /// <param name="userId"></param>
        /// <returns>document created</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task CreateFolderAsync(Folder folder)
        {
            // Valider at ParentFolderId (hvis angitt) tilhører samme bruker
            if (folder.ParentFolderId.HasValue)
            {
                var parentFolder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folder.ParentFolderId && f.UserId == folder.UserId);
                
                if (parentFolder == null)
                {
                    throw new ArgumentException("Overordnet mappe ble ikke funnet eller tilhører ikke brukeren.");
                }
            }
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
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
            var folder = await _context.Folders
                .Include(f => f.ChildrenFolders)
                .Include(f => f.Documents)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            
            if (folder == null)
            {
                return false;
            }

            // Slett alle undermapper og dokumenter rekursivt
            DeleteFolderRecursive(folder);

            await _context.SaveChangesAsync();
            return true;
        }
        
        private void DeleteFolderRecursive(Folder folder)
        {
            // Slett alle dokumenter i mappen
            _context.Documents.RemoveRange(folder.Documents);

            // Hent undermapper
            foreach (var childFolder in folder.ChildrenFolders)
            {
                // Rekursivt slett undermapper
                DeleteFolderRecursive(childFolder);
            }

            // Slett mappen
            _context.Folders.Remove(folder);
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
