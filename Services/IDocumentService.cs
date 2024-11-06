using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS_Project.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetAllDocumentsAsync();
        Task<Document> GetDocumentByIdAsync(int id);
        Task<Document> CreateDocumentAsync(DocumentDto documentDto);
        Task<bool> DeleteDocumentAsync(int id);
        Task<bool> UpdateDocumentAsync(int id, UpdateDocumentDto updateDocumentDto, int userId);

    }
}