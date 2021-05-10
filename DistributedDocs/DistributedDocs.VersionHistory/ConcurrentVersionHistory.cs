using System;
using System.Collections.Generic;
using System.Linq;

namespace DistributedDocs.VersionHistory
{
    public sealed class ConcurrentVersionHistory<T> : IConcurrentVersionHistory<T> where T : notnull
    {
        private readonly object _syncRoot = new object();
        private readonly SortedSet<ICommit<T>> _history = new SortedSet<ICommit<T>>();
        private readonly IAuthorInfo _self;
        private readonly ICommitMerger<T> _commitMerger;

        internal ConcurrentVersionHistory(IAuthorInfo self, string name, ICommitMerger<T> commitMerger)
        {
            _self = self;
            Name = name;
            _commitMerger = commitMerger;
        }

        public ConcurrentVersionHistory(IAuthorInfo self, string name, ICommitMerger<T> commitMerger, Guid historyId, IEnumerable<ICommit<T>> existingCommits)
            : this(self, name, commitMerger)
        {
            Guid = historyId;
            foreach (var commit in existingCommits)
            {
                SaveCommit(commit);
            }
        }

        public Guid Guid { get; } = Guid.NewGuid();
        public string Name { get; }
        public IReadOnlyCollection<ICommit<T>> History => _history;

        public ICommit<T> CommitChange(T change)
        {
            lock (_syncRoot)
            {
                _history.Reverse();
                var commit = new Commit<T>(_history.LastOrDefault()?.Id + 1 ?? 0, _self, change);
                SaveCommit(commit);
                return commit;
            }
        }

        public ICommit<T>? AddCommit(ICommit<T> commit)
        {
            lock (_syncRoot)
            {
                var corrected = ResolveConflicts(commit);
                SaveCommit(corrected);
                return corrected.Id == commit.Id ? null : corrected;
            }
        }

        private void SaveCommit(ICommit<T> commit)
        {
            _history.Add(commit);
            _commitMerger.Merge(_history, commit);
        }

        private ICommit<T> ResolveConflicts(ICommit<T> commit)
        {
            var lastId = -1;
            if (_history.Any())
            {
                lastId = _history.Last().Id;
            }

            if (lastId == commit.Id)
            {
                commit = commit.UpdateId(commit.Id + 1);
            }

            _history.Add(commit);
            return commit;
        }
    }
}