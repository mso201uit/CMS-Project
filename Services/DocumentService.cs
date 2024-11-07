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

        /// <summary>
        /// GET all documents where UserId = Documents-User.Id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>List of Documents by given user id</returns>
        public async Task<IEnumerable<Document>> GetAllDocumentsAsync(int UserId)
        {
            return await _context.Documents
                .Include(d => d.User)
                .Include(d => d.Folder)
                .Where(d => d.UserId == UserId)
                .ToListAsync();
        }
        
        /// <summary>
        /// GET document by id given
        /// </summary>
        /// <param name="id"></param>
        /// <returns>document by id given</returns>
        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.User)
                .Include(d => d.Folder)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// CREATE document by Dto and checks ownership
        /// </summary>
        /// <param name="documentDto"></param>
        /// <param name="userId"></param>
        /// <returns>document created</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Document> CreateDocumentAsync(DocumentDto documentDto, int userId)
        {
            // checks if user and folder exists
            var folder = await _context.Folders.FindAsync(documentDto.FolderId);

            if (folder == null)
                throw new ArgumentException("folder not found.");
            // checks if user owns the folder
            if (folder.UserId != userId)
            {
                throw new ArgumentException("User doesn't own the folder.");
            }
            var document = new Document
            {
                Title = documentDto.Title,
                Content = documentDto.Content,
                ContentType = documentDto.ContentType,
                CreatedDate = DateTime.UtcNow,
                UserId = userId,
                FolderId = documentDto.FolderId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }

        /// <summary>
        /// DELETE document by given id and checks ownership
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>true when deleted and false if failed</returns>
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

        /// <summary>
        /// UPDATE document by given id and checks ownership. Updates by dto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDocumentDto"></param>
        /// <param name="userId"></param>
        /// <returns>true if completed, false if not</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
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

        /// <summary>
        /// checks if document with id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true if exists, and false if not</returns>
        private async Task<bool> DocumentExists(int id)
        {
            return await _context.Documents.AnyAsync(e => e.Id == id);
        }
    }
}
