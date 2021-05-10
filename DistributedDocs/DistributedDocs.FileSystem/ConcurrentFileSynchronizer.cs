using DistributedDocs.DocumentChanges;
using System;
using System.IO;
using System.Text;

namespace DistributedDocs.FileSystem
{
    public sealed class ConcurrentFileSynchronizer : IConcurrentFileSynchronizer<ITextDiff>
    {
        private readonly string _path;
        public ConcurrentFileSynchronizer(string path)
        {
            _path = path;
        }

        public void AddChange(ITextDiff textDiff)
        {
            var content = new StringBuilder(File.ReadAllText(_path));

            Delete(textDiff.StartIndex, textDiff.EndIndex, content);
            Insert(textDiff.StartIndex, textDiff.Text, content);

            File.WriteAllText(_path, content.ToString());
        }

        private void Delete(int from, int to, StringBuilder content)
        {
            if (from == to)
            {
                return;
            }

            if (from < 0 || from > content.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            if (to < 0 || to > content.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            content.Remove(from, to - from);
        }

        private void Insert(int from, string text, StringBuilder content)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (from < 0 || from > content.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            content.Insert(from, text);
        }

    }
}