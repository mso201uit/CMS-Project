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

        //GET all documents where UserId = Documents-User.Id
        public async Task<IEnumerable<Document>> GetAllDocumentsAsync(int UserId)
        {
            return await _context.Documents
                .Include(d => d.User)
                .Include(d => d.Folder)
                .Where(d => d.UserId == UserId)
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
            // checks if user and folder exists
            var user = await _context.Users.FindAsync(documentDto.UserId);
            var folder = await _context.Folders.FindAsync(documentDto.FolderId);

            if (user == null)
                throw new ArgumentException("User not found.");

            if (folder == null)
                throw new ArgumentException("folder not found.");
            // checks if user owns the folder
            if (folder.UserId != user.Id)
            {
                throw new ArgumentException("User doesn't own the folder.");
            }
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

        public async Task<bool> DeleteDocumentAsync(int id, int userId)
        {
            //get document by id
            var document = await _context.Documents.FindAsync(id);
            //check if it's found
            if (document == null)
                return false;
            //checks if document is owned by the user
            if (userId == document.UserId)
            {
                //proceed with deleting
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateDocumentAsync(int id, UpdateDocumentDto updateDocumentDto, int userId)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            // check if document belongs to user
            if (document.UserId != userId)
                throw new UnauthorizedAccessException("User doesn't have access to this document.");
            // check if folder belongs to user
            var folder = await _context.Folders.FindAsync(updateDocumentDto.FolderId);
            if (folder.UserId != userId)
                throw new UnauthorizedAccessException("User doesn't have access to this folder.");


            // update properties
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
