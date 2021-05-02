namespace DistributedDocs.VersionHistory
{
    public sealed class Commit<T> : ICommit<T> where T : notnull
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
    }
}