namespace DistributedDocs.VersionHistory
{
    public interface IVersionHistoryProvider<T> where T : notnull
    {
        IConcurrentVersionHistory<T> ProvideHistory(string name, string? path = null);
    }
}
