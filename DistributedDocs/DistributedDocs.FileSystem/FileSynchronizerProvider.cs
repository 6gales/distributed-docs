using System.Text;
using System.IO;
using DistributedDocs.DocumentChanges;

namespace DistributedDocs.FileSystem
{
    public sealed class FileSynchronizerProvider : IFileSynchronizerProvider<ITextDiff>
    {
        public IConcurrentFileSynchronizer<ITextDiff> ProvideFileSynchronizer(string name, string? path)
        {
            path = path != null ? $"{path}//{name}" : name;
            var stream = new FileStream(path, FileMode.OpenOrCreate);
            
            return new ConcurrentFileSynchronizer(stream, Encoding.Unicode);
        }
    }
}