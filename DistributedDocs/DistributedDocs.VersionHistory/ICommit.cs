using System;

namespace DistributedDocs.VersionHistory
{
    public interface ICommit<T> : IComparable<ICommit<T>>
        where T : notnull
    {
        int Id { get; }

        IAuthorInfo Author { get; }

        T Change { get; }

        ICommit<T> UpdateId(int newId);

        public static bool operator >(ICommit<T> a, ICommit<T> b) => a.CompareTo(b) > 0;

        public static bool operator <(ICommit<T> a, ICommit<T> b) => a.CompareTo(b) < 0;

        public static bool operator >=(ICommit<T> a, ICommit<T> b) => a.CompareTo(b) >= 0;

        public static bool operator <=(ICommit<T> a, ICommit<T> b) => a.CompareTo(b) <= 0;
    }
}