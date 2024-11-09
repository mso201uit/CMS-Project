using CMS_Project.Models;
using CMS_Project.Models.DTOs;

namespace CMS_Project.Services
{
    public interface IFolderService
    {
        Task CreateFolderAsync(Folder folder);
        Task<List<FolderDto>> GetAllFoldersAsDtoAsync(int userId);
        Task<Folder> GetFolderByIdAsync(int id);
        //Task<IEnumerable<Folder>> GetAllFoldersAsync(int UserId);
        Task<bool> UpdateFolderAsync(int id, UpdateFolderDto updateFolderDto, int userId);
        Task<bool> DeleteFolderAsync(int id, int userId);
        Task<IEnumerable<Folder>> GetFoldersByUserIdAsync(int userId);

    }
}