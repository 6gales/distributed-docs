namespace DistributedDocs.VersionHistory
{
    public interface ICommit<out T> where T : notnull
    {
        int Id { get; }

        IAuthorInfo Author { get; }

        T Change { get; }

        ICommit<T> UpdateId(int newId);
    }
}
