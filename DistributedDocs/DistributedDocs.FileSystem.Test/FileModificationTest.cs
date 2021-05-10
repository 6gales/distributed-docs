using DistributedDocs.DocumentChanges;
using NUnit.Framework;
using System.IO;

namespace DistributedDocs.FileSystem.Test
{
    [TestFixture]
    public class Tests
    {
        private const string Name = "test.txt";

        [TearDown]
        public void OnceTimeTearDown()
        {
            File.Delete(Name);
        }

        [Test]
        public void InsertTest()
        {
            File.WriteAllText(Name, "test");
            var diff  = new TextDiff(1, 1, "a");
            var synchronizer = new ConcurrentFileSynchronizer(Name);
            synchronizer.AddChange(diff);

            Assert.AreEqual("taest", File.ReadAllText(Name));
        }

        [Test]
        public void ProviderTest()
        {
            var provider = new FileSynchronizerProvider();
            var sync = provider.ProvideFileSynchronizer(Name, "");
            var diff = new TextDiff(0, 0, "a");
            sync.AddChange(diff);

            Assert.AreEqual("a", File.ReadAllText(Name));
        }

        [Test]
        public void DeleteTest()
        {
            File.WriteAllText(Name, "test");
            var diff = new TextDiff(1, 2, "");
            var synchronizer = new ConcurrentFileSynchronizer(Name);
            synchronizer.AddChange(diff);

            Assert.AreEqual("tst", File.ReadAllText(Name));
        }

        [Test]
        public void ReplaceTest()
        {
            File.WriteAllText(Name, "test");
            var diff = new TextDiff(1, 2, "a");
            var synchronizer = new ConcurrentFileSynchronizer(Name);
            synchronizer.AddChange(diff);

            Assert.AreEqual("tast", File.ReadAllText(Name));
        }
    }
}