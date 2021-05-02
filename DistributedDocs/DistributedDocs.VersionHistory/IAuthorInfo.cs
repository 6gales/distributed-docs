using System;

namespace DistributedDocs.VersionHistory
{
    public interface IAuthorInfo
    {
        Guid Guid { get; }

        string Name { get; }
    }
}
