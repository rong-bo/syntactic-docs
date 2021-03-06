using System.Collections.Generic;
using System.Linq;
using SyntacticDocs.Models;
using SyntacticDocs.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace SyntacticDocs.Services
{
    public class DocumentService
    {
        private readonly ApplicationDbContext _db;
        private readonly SearchService _searchService;

        public DocumentService(ApplicationDbContext db,SearchService searchService)
        {
            _db = db;            
            _db.SeedData();
            _searchService = searchService;
        }

        public Document GetDocument(string alias)
        {
            return _db.Docs
                .Include(doc => doc.Documents)
                .ThenInclude(doc => doc.Documents)
                .FirstOrDefault(doc => doc.Alias==alias);                
        }

        public IEnumerable<Document> GetRelatedDocuments(Document document)
        {
            return _db.Docs.Where(doc=>doc.Tags.Any(tag=>document.Tags.Contains(tag)));
        }

        public Document Add(Document document, Guid parentId)
        {
            var parentDocument = _db.Docs.FirstOrDefault(doc=>doc.Id==parentId);
            parentDocument.Documents.Add(document);
            _db.SaveChanges();
            _searchService.ImportData();
            return document;
        }
        public Document Save(Document document)
        {
            var oldDocument = _db.Docs.FirstOrDefault(doc=>doc.Id==document.Id);
            oldDocument.Content = document.Content;
            _db.SaveChanges();
            _searchService.ImportData();
            return oldDocument;
        }

        public Document Delete(Document document)
        {
            var oldDocument = _db.Docs.FirstOrDefault(doc=>doc.Id==document.Id);
            _db.Remove(oldDocument);
            _db.SaveChanges();
            _searchService.ImportData();
            return document;
        }
    }
}