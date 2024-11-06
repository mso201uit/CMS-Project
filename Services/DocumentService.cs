using CMS_Project.Data;
using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CMS_Project.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly CMSContext _context;

        public DocumentService(CMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _context.Documents
                .Include(d => d.User)
                .Include(d => d.Folder)
                .ToListAsync();
        }
        
        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.User)
                .Include(d => d.Folder)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Document> CreateDocumentAsync(DocumentDto documentDto)
        {
            // Sjekk om bruker og mappe eksisterer
            var user = await _context.Users.FindAsync(documentDto.UserId);
            var folder = await _context.Folders.FindAsync(documentDto.FolderId);

            if (user == null)
                throw new ArgumentException("Bruker ikke funnet.");

            if (folder == null)
                throw new ArgumentException("Mappe ikke funnet.");

            var document = new Document
            {
                Title = documentDto.Title,
                Content = documentDto.Content,
                ContentType = documentDto.ContentType,
                CreatedDate = DateTime.UtcNow,
                UserId = documentDto.UserId,
                FolderId = documentDto.FolderId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateDocumentAsync(int id, UpdateDocumentDto updateDocumentDto, int userId)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            // Sjekk om dokumentet tilhører brukeren
            if (document.UserId != userId)
                throw new UnauthorizedAccessException("Du har ikke tilgang til dette dokumentet.");

            // Oppdater egenskaper
            document.Title = updateDocumentDto.Title;
            document.Content = updateDocumentDto.Content;
            document.ContentType = updateDocumentDto.ContentType;
            document.FolderId = updateDocumentDto.FolderId;

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DocumentExists(id))
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

        private async Task<bool> DocumentExists(int id)
        {
            return await _context.Documents.AnyAsync(e => e.Id == id);
        }
    }
}
