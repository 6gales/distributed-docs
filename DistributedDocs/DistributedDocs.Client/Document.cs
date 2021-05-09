using System;
using System.Collections.Generic;

namespace DistributedDocs.Client
{
    namespace Data
    {
        public sealed class Document
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Content { get; set; }
        }

        public static class Documents
        {
            private static readonly List<Document> documents = new List<Document>
            {
                new Document {Name = "Test", Content = "It is content"}
            };
            
            public static List<Document> GetDocuments() 
            {
                return documents;
            }
            public static void UpdateDocument(Document document)
            {
                var id = documents.IndexOf(document);
                documents[id] = document;
            }
            public static int AddDocument(Document document) 
            {
                documents.Add(document);
                return documents.Count - 1;
            }
            public static Document? GetDocument(int id) 
            {
                Console.Write("ewfewfefg" + id);
                if (id >= documents.Count || id < 0)
                {
                    return null;
                }
                return documents[id];//.Find(n => n.Id.Equals(id));
            }
            public static string Url(int id)
            { 
                return "documents/" + id;
            }
        }
    }
}