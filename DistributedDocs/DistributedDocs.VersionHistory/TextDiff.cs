namespace DistributedDocs.VersionHistory
{
    internal enum DiffType
    {
        Add,
        Delete,
        Replace
    }

    public sealed class TextDiff
    {
        private readonly DiffType _diffType;

        private TextDiff(DiffType type, int id, string author, int startIndex, int endIndex, string text)
        {
            _diffType = type;
            Id = id;
            Author = author;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Text = text;
        }

        public int Id { get; }

        public string Author { get; }

        public int StartIndex { get; }

        public int EndIndex { get; }

        public string Text { get; }
    }
}