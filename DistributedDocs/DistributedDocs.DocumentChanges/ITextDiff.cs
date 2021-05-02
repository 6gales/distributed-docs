namespace DistributedDocs.DocumentChanges
{
    public interface ITextDiff
    {
        DiffType DiffType { get; }

        int StartIndex { get; }

        int EndIndex { get; }

        string Text { get; }
    }
}
