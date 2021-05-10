using System.Text;
using System.IO;
using DistributedDocs.DocumentChanges;

namespace DistributedDocs.FileSystem
{
    public sealed class FileSynchronizerProvider : IFileSynchronizerProvider<ITextDiff>
    {
        public IConcurrentFileSynchronizer<ITextDiff> ProvideFileSynchronizer(string name, string? path)
        {
            path = !string.IsNullOrWhiteSpace(path) ? $"{path}//{name}" : name;
            if (!File.Exists(path))
            {
                using var stream = File.Create(path);
            }
            
            return new ConcurrentFileSynchronizer(path);
        }
    }
}