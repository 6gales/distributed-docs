namespace DistributedDocs.DocumentChanges
{
    public sealed class TextDiff : ITextDiff
    {
        public TextDiff(int startIndex, int endIndex, string text)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Text = text;
        }

        public DiffType DiffType => GetDiffType();

        public int StartIndex { get; }

        public int EndIndex { get; }

        public string Text { get; }

        private DiffType GetDiffType()
        {
            if (StartIndex == EndIndex)
            {
                return DiffType.Add;
            }

            return string.IsNullOrEmpty(Text) ? DiffType.Delete : DiffType.Replace;
        }
    }
}
