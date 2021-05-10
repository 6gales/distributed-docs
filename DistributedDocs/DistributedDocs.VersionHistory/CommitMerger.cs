using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistributedDocs.DocumentChanges;
using DistributedDocs.FileSystem;

namespace DistributedDocs.VersionHistory
{
    public sealed class CommitMerger : ICommitMerger<ITextDiff>
    {
        private readonly IConcurrentFileSynchronizer<ITextDiff> _fileSynchronizer;

        public CommitMerger(IConcurrentFileSynchronizer<ITextDiff> fileSynchronizer)
        {
            _fileSynchronizer = fileSynchronizer;
        }

        public void Merge(IReadOnlyCollection<ICommit<ITextDiff>> history, ICommit<ITextDiff> newCommit)
        {
            IReadOnlyList<ITextDiff> forwardedCommits = history
                .Where(commit => commit > newCommit)
                .Select(commit => commit.Change)
                .ToList();

            var copiedHistory = new LinkedList<ITextDiff>(
                history.Where(commit => commit != newCommit).Select(commit => commit.Change));

            var reversedCommits = forwardedCommits.Reverse().Select(diff => ReverseChange(copiedHistory, diff));
            foreach (var revertedDiff in reversedCommits)
            {
                _fileSynchronizer.AddChange(revertedDiff);
            }

            _fileSynchronizer.AddChange(newCommit.Change);
            foreach (var reappliedCommit in forwardedCommits)
            {
                _fileSynchronizer.AddChange(reappliedCommit);
            }
        }

        private static ITextDiff ReverseChange(LinkedList<ITextDiff> history, ITextDiff change)
        {
                history.RemoveLast();
            return new TextDiff(
                change.StartIndex,
                change.StartIndex + change.Text.Length,
                RestoreText(history, change.StartIndex, change.EndIndex));
        }

        private static string RestoreText(IEnumerable<ITextDiff> history, int from, int to)
        {
            if (from == to)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var textDiff in history)
            {
                builder
                    .Remove(textDiff.StartIndex, textDiff.EndIndex - textDiff.StartIndex)
                    .Insert(textDiff.StartIndex, textDiff.Text);
            }

            return builder.ToString(from, to - from);
        }
    }
}