using System;
using System.Collections.Generic;
using System.Linq;
using DistributedDocs.DocumentChanges;
using DistributedDocs.FileSystem;

namespace DistributedDocs.VersionHistory
{
    public sealed class Merger
    {
        private readonly IConcurrentFileSynchronizer<ITextDiff> _fileSynchronizer;

        public Merger(IConcurrentFileSynchronizer<ITextDiff> fileSynchronizer)
        {
            _fileSynchronizer = fileSynchronizer;
        }

        public void Merge(IEnumerable<ICommit<ITextDiff>> history, ICommit<ITextDiff> newCommit)
        {
            var commitGroups = history
                .GroupBy(commit => commit.CompareTo(newCommit), commit => commit.Change)
                .OrderBy(group => group.Key)
                .ToList();
           
            var previousCommits = commitGroups.First().ToArray();
            var forwardedCommits = commitGroups.Last().ToArray();

            var reversedCommits = forwardedCommits.Reverse().Select(diff => ReverseChange(previousCommits, diff));
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

        private ITextDiff ReverseChange(IReadOnlyList<ITextDiff> history, ITextDiff change)
        {
            return change.DiffType switch
            {
                DiffType.Add => new TextDiff(change.StartIndex, change.EndIndex, string.Empty),
                DiffType.Delete => new TextDiff(
                    change.StartIndex,
                    change.StartIndex,
                    RestoreText(history, change.StartIndex, change.EndIndex)),
                DiffType.Replace => new TextDiff(
                    change.StartIndex,
                    change.StartIndex + change.Text.Length,
                    RestoreText(history, change.StartIndex, change.EndIndex)),
                _ => throw new ArgumentOutOfRangeException(nameof(change.DiffType))
            };
        }

        private string RestoreText(IReadOnlyList<ITextDiff> history, int from, int to)
        {
            for (int i = history.Count - 1; i >= 0; i--)
            {
                //history[i].
            }
        }
    }
}