namespace DistributedDocs.VersionHistory
{
    public sealed class Commit<T> : ICommit<T>
        where T : notnull
    {
        internal Commit(int id, IAuthorInfo author, T change)
        {
            Id = id;
            Author = author;
            Change = change;
        }

        public int Id { get; }

        public IAuthorInfo Author { get; }

        public T Change { get; }

        public ICommit<T> UpdateId(int newId)
        {
            return new Commit<T>(newId, Author, Change);
        }

        public int CompareTo(ICommit<T>? other)
        {
            if (other is null)
            {
                return 1;
            }

            var comparison = Id.CompareTo(other.Id);
            return comparison != 0 ? comparison : Author.Guid.CompareTo(other.Author.Guid);
        }
    }
}