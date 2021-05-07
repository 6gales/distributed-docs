using System.Text;
using System.IO;

namespace DistributedDocs.FileSystem
{
    public sealed class FileSynchronizerProvider : IFileSynchronizerProvider
    {
        public IConcurrentFileSynchronizer Provide(string name, string? path = null)
        {
            path = path != null ? $"{path}//{name}" : name;
            var stream = new FileStream(path, FileMode.OpenOrCreate);
            
            return new ConcurrentFileSynchronizer(stream, Encoding.Unicode);
        }
    }
}
