using System;
using System.Collections.Generic;
using DistributedDocs.Server.ClientModels;

namespace DistributedDocs.Client
{
    internal static class DocumentManager
    {
        private static readonly Dictionary<Guid, DocumentInfo> Documents = new Dictionary<Guid, DocumentInfo>
        {
            [Guid.Empty] = new DocumentInfo {DocumentId = Guid.Empty, DocumentName = "aaaa"}
        };

        public static IReadOnlyCollection<DocumentInfo> DocumentsInfos => Documents.Values;

        public static void StoreDocuments(IEnumerable<DocumentInfo> documents)
        {
            Documents.Clear();
            foreach (var document in documents)
            {
                Documents.Add(document.DocumentId, document);
            }
        }

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
            return $"documents/{documentId}";
        }
    }
}