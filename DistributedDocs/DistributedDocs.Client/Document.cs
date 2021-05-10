using System;
using System.Collections.Generic;
using System.Linq;
using DistributedDocs.Server.ClientModels;

namespace DistributedDocs.Client
{
    public static class DocumentManager
    {
        private static readonly Dictionary<Guid, DocumentInfo> Documents = new Dictionary<Guid, DocumentInfo>();

        public static IReadOnlyCollection<DocumentInfo> DocumentsInfos => Documents.Values;
        
        public static void AddDocument(DocumentInfo document) 
        {
            Documents.Add(document.DocumentId, document);
        }
        public static DocumentInfo? GetDocument(Guid documentId) 
        {
            if (Documents.TryGetValue(documentId, out var info))
            {
                return info;
            }
            return null;
        }
        public static string Url(Guid documentId)
        { 
            return "documents/" + Documents
                .Where(kv => kv.Key == documentId)
                .Select((kv, i) => i)
                .FirstOrDefault();
        }
    }
}