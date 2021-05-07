using System;

namespace DistributedDocs.VersionHistory
{
    public interface IAuthorInfoEditor : IAuthorProvider
    {
        Guid Guid { get; }

        string? Name { get; set; }
    }
}