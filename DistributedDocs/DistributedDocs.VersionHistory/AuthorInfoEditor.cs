using System;

namespace DistributedDocs.VersionHistory
{
    public sealed class AuthorInfoEditor : IAuthorInfoEditor
    {
        public AuthorInfoEditor()
        {
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; }

        public string? Name { get; set; }

        public IAuthorInfo ProvideAuthor()
        {
            var name = Name ?? "Anonymous";
            return new AuthorInfo(Guid, name);
        }

    }
}