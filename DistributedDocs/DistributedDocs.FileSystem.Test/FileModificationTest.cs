using DistributedDocs.DocumentChanges;
using NUnit.Framework;
using System.IO;

namespace DistributedDocs.FileSystem.Test
{
    public class Tests
    {
        private const string _name = "test.txt";

        [TearDown]
        public void OnceTimeTearDown()
        {
            File.Delete(_name);
        }

        [Test]
        public void InsertTest()
        {
            File.WriteAllText(_name, "test");
            var diff  = new TextDiff(1, 1, "a");
            var synchronizer = new ConcurrentFileSynchronizer(_name);
            synchronizer.AddChange(diff);

            Assert.AreEqual("taest", File.ReadAllText(_name));
        }

        [Test]
        public void ProviderTest()
        {
            var provider = new FileSynchronizerProvider();
            var sync = provider.ProvideFileSynchronizer(_name, "");
            var diff = new TextDiff(0, 0, "a");
            sync.AddChange(diff);

            Assert.AreEqual("a", File.ReadAllText(_name));
        }

        [Test]
        public void DeleteTest()
        {
            File.WriteAllText(_name, "test");
            var diff = new TextDiff(1, 2, "");
            var synchronizer = new ConcurrentFileSynchronizer(_name);
            synchronizer.AddChange(diff);

            Assert.AreEqual("tst", File.ReadAllText(_name));
        }

        [Test]
        public void ReplaceTest()
        {
            File.WriteAllText(_name, "test");
            var diff = new TextDiff(1, 2, "a");
            var synchronizer = new ConcurrentFileSynchronizer(_name);
            synchronizer.AddChange(diff);

            Assert.AreEqual("tast", File.ReadAllText(_name));
        }
    }
}