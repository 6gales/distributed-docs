using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public (int, int, string) Deconstruct()
        {
            return (StartIndex, EndIndex, Text);
        }

        public static string TextFrom(IEnumerable<ITextDiff> history)
        {
            var builder = new StringBuilder();
            foreach (var textDiff in history)
            {
                builder
                    .Remove(textDiff.StartIndex, textDiff.EndIndex - textDiff.StartIndex)
                    .Insert(textDiff.StartIndex, textDiff.Text);
            }

            return builder.ToString();
        }

        public static IReadOnlyCollection<ITextDiff> BetweenTexts(string source, string dest)
        {
            var (distance, d) = MatrixBetweenTexts(source, dest);
            var m = source.Length;
            var n = dest.Length;
            var edits = new List<ITextDiff>();

            while (m > 0 || n > 0)
            {
                var del = m >= 1 ? d[m - 1, n] : int.MaxValue;
                var ins = n >= 1 ? d[m, n - 1] : int.MaxValue;
                var subs = m >= 1 && n >= 1 ? d[m - 1, n - 1] : int.MaxValue;
                var smallest = Math.Min(del, Math.Min(ins, subs));
                if (smallest == subs)
                {
                    if (subs < d[m, n])
                    {
                        edits.Add(new TextDiff(n - 1, n, dest[n - 1].ToString()));
                    }

                    m--;
                    n--;
                }
                else if (smallest == del)
                {
                    edits.Add(new TextDiff(n, n + 1, string.Empty));
                    m--;
                }
                else if (smallest == ins)
                {
                    edits.Add(new TextDiff(n - 1, n - 1, dest[n - 1].ToString()));
                    n--;
                }
            }

            if (distance != edits.Count)
            {
                throw new Exception("Distance and number of edits mismatch");
            }

            edits.Reverse();
            return edits;
        }

        public static ITextDiff SquashTextDiffs(IReadOnlyList<ITextDiff> edits)
        {
            var diffType = edits.Aggregate(
                edits[0].DiffType,
                (prevType, diff) => prevType == diff.DiffType ? prevType : DiffType.Replace);

            return diffType switch
            {
                DiffType.Add => new TextDiff(
                    edits[0].StartIndex,
                    edits[0].StartIndex,
                    edits.Aggregate(string.Empty, (text, diff) => text + diff.Text)),
                DiffType.Delete => new TextDiff(
                    edits[0].StartIndex,
                    edits[^1].EndIndex,
                    string.Empty),
                _ => new TextDiff(
                    edits[0].StartIndex,
                    edits.Last(diff => diff.DiffType != DiffType.Add).EndIndex,
                    edits.Aggregate(string.Empty, (text, diff) => text + diff.Text))
            };
        }

        private static (int, int[,]) MatrixBetweenTexts(string source, string dest)
        {
            var d = new int[source.Length + 1, dest.Length + 1];
            for (var i = 1; i < source.Length + 1; i++)
            {
                d[i, 0] = i;
            }

            for (var j = 1; j < dest.Length + 1; j++)
            {
                d[0, j] = j;
            }

            for (var j = 1; j < dest.Length + 1; j++)
            {
                for (var i = 1; i < source.Length + 1; i++)
                {
                    var substitutionCost = source[i - 1] == dest[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(d[i - 1, j] + 1, Math.Min(d[i, j - 1] + 1, d[i - 1, j - 1] + substitutionCost));
                }
            }

            return (d[source.Length, dest.Length], d);
        }
    }
}