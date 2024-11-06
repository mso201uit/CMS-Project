﻿using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS_Project.Services
{
    public interface IFolderService
    {
        Task<IEnumerable<Folder>> GetAllFoldersAsync();
        Task<Folder> GetFolderByIdAsync(int id);
        Task<Folder> CreateFolderAsync(FolderDto folderDto);
        Task<bool> UpdateFolderAsync(int id, UpdateFolderDto updateFolderDto);
        Task<bool> DeleteFolderAsync(int id);
    }
}