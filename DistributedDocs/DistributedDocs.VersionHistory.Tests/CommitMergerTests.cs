using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DistributedDocs.DocumentChanges;
using DistributedDocs.FileSystem;
using Moq;
using NUnit.Framework;

namespace DistributedDocs.VersionHistory.Tests
{
    [TestFixture]
    [Localizable(false)]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class CommitMergerTests
    {
        private sealed class StringBuilderSynchronizer : IConcurrentFileSynchronizer<ITextDiff>
        {
            private readonly StringBuilder _builder = new StringBuilder();

            public void AddChange(ITextDiff change)
            {
                _builder.Remove(change.StartIndex, change.EndIndex - change.StartIndex)
                    .Insert(change.StartIndex, change.Text);
            }

            public string Text => _builder.ToString();
        }

        [Test]
        public void FormHistoryTest()
        {
            var sync = new StringBuilderSynchronizer();
            var merger = new CommitMerger(sync);
            var author = Mock.Of<IAuthorInfo>();
            var id = 0;

            var commit1 = new Commit<ITextDiff>(id++, author, new TextDiff(0, 0, "aaaa"));
            var history = new SortedSet<ICommit<ITextDiff>> {commit1};

            merger.Merge(history, commit1);
            Assert.That(sync.Text, Is.EqualTo("aaaa"));

            var commit2 = new Commit<ITextDiff>(id++, author, new TextDiff(0, 0, "bb"));
            history.Add(commit2);
            merger.Merge(history, commit2);
            Assert.That(sync.Text, Is.EqualTo("bbaaaa"));

            var commit3 = new Commit<ITextDiff>(id++, author, new TextDiff(2, 4, "cccc"));
            history.Add(commit3);
            merger.Merge(history, commit3);
            Assert.That(sync.Text, Is.EqualTo("bbccccaa"));

            var commit6 = new Commit<ITextDiff>(id + 2, author, new TextDiff(4, 6, string.Empty));
            history.Add(commit6);
            merger.Merge(history, commit6);
            Assert.That(sync.Text, Is.EqualTo("bbccaa"));

            var commit7 = new Commit<ITextDiff>(id + 3, author, new TextDiff(6, 6, "eeee"));
            history.Add(commit7);
            merger.Merge(history, commit7);
            Assert.That(sync.Text, Is.EqualTo("bbccaaeeee"));

            var commit4 = new Commit<ITextDiff>(id, author, new TextDiff(4, 4, "LLLL"));
            history.Add(commit4);
            merger.Merge(history, commit4);
            Assert.That(sync.Text, Is.EqualTo("bbccLLeeeeccaa"));

            var commit5 = new Commit<ITextDiff>(id + 1, author, new TextDiff(8, 10, "RRRR"));
            history.Add(commit5);
            merger.Merge(history, commit5);
            Assert.That(sync.Text, Is.EqualTo("bbccLLeeeeRRRRaa"));
        }
    }
}