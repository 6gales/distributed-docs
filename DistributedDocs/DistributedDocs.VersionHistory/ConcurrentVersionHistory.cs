using System;
using System.Collections.Generic;
using System.Linq;
using DistributedDocs.FileSystem;

namespace DistributedDocs.VersionHistory
{
    public sealed class ConcurrentVersionHistory<T> : IConcurrentVersionHistory<T> where T : notnull
    {
        private readonly object _syncRoot = new object();
        private readonly SortedSet<ICommit<T>> _history = new SortedSet<ICommit<T>>();
        private readonly IAuthorInfo _self;
        private readonly IConcurrentFileSynchronizer<T> _fileSynchronizer;

        internal ConcurrentVersionHistory(IAuthorInfo self, IConcurrentFileSynchronizer<T> fileSynchronizer)
        {
            _self = self;
            _fileSynchronizer = fileSynchronizer;
        }

        public Guid Guid { get; } = Guid.NewGuid();

        public ICommit<T> CommitChange(T change)
        {
            lock (_syncRoot)
            {
                _history.Reverse();
                var commit = new Commit<T>(_history.LastOrDefault()?.Id ?? 0, _self, change);
                _history.Add(commit);
                _fileSynchronizer.AddChange(change);
                return commit;
            }
        }

        public ICommit<T>? AddCommit(ICommit<T> commit)
        {
            lock (_syncRoot)
            {
                var corrected = ResolveConflicts(commit);
                _history.Add(corrected);
                _fileSynchronizer.AddChange(corrected.Change);
                return corrected.Id == commit.Id ? null : corrected;
            }
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