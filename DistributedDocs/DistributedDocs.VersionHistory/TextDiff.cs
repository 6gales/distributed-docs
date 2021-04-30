namespace DistributedDocs.VersionHistory
{
    public enum DiffType
    {
        Add,
        Delete,
        Replace
    }

    public sealed class TextDiff : ITextDiff
    {
        private TextDiff(DiffType type, int id, string author, int startIndex, int endIndex, string text)
        {
            DiffType = type;
            Id = id;
            Author = author;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Text = text;
        }

        public DiffType DiffType { get; }

        public int Id { get; }

        public string Author { get; }

        public int StartIndex { get; }

        public int EndIndex { get; }

        public string Text { get; }

        public static ITextDiff Add(int id, string author, int index, string text)
        {
            return new TextDiff(DiffType.Add, id, author, index, index, text);
        }

        public static ITextDiff Delete(int id, string author, int from, int to)
        {
            return new TextDiff(DiffType.Delete, id, author, from, to, string.Empty);
        }

        public static ITextDiff Replace(int id, string author, int from, int to, string text)
        {
            return new TextDiff(DiffType.Replace, id, author, from, to, text);
        }
    }
}