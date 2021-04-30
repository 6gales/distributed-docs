namespace DistributedDocs.VersionHistory
{
    public interface ITextDiff
    {
        public DiffType DiffType { get; }

        public int Id { get; }

        public string Author { get; }

        public int StartIndex { get; }

        public int EndIndex { get; }

        public string Text { get; }
    }
}
