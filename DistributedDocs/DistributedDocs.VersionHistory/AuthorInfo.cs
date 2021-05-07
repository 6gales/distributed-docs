using System;

namespace DistributedDocs.VersionHistory
{
    public sealed class AuthorInfo : IAuthorInfo
    {
        public AuthorInfo(Guid guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public Guid Guid { get; }
        public string Name { get; }
    }
}