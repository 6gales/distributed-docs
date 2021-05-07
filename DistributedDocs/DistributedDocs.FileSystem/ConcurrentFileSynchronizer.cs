using DistributedDocs.VersionHistory;
using System;
using System.IO;
using System.Text;

namespace DistributedDocs.FileSystem
{
    public sealed class ConcurrentFileSynchronizer : IConcurrentFileSynchronizer
    {
        private readonly FileStream _fileStream;
        private readonly Encoding _encoding;
        private readonly int _charSize;

        public ConcurrentFileSynchronizer(FileStream stream, Encoding encoding)
        {
            _fileStream = stream;
            _encoding = encoding;
            _charSize = encoding.GetByteCount("b");
        }

        public void AddCommit(ITextDiff textDiff)
        {
            Delete(textDiff.StartIndex, textDiff.EndIndex);
            Insert(textDiff.StartIndex, textDiff.Text);
        }

        private void Delete(int from, int to)
        {
            if (from == to)
            {
                return;
            }

            if (from < 0 || from > _fileStream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            if (to < 0 || to > _fileStream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }


        }

        private void Insert(int from, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (from < 0 || from > _fileStream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }
            const int maxBufferSize = 1024 * 512;
            int bufferSize = maxBufferSize;
            long temp = _fileStream.Length - from;
            if (temp <= maxBufferSize)
            {
                bufferSize = (int)temp;
            }
            byte[] buffer = new byte[bufferSize];
            long currentPositionToRead = _fileStream.Length;
            int numberOfBytesToRead;
            while (true)
            {
                numberOfBytesToRead = bufferSize;
                temp = currentPositionToRead - from;
                if (temp < bufferSize)
                {
                    numberOfBytesToRead = (int)temp;
                }
                currentPositionToRead -= numberOfBytesToRead;
                _fileStream.Position = currentPositionToRead;
                _fileStream.Read(buffer, 0, numberOfBytesToRead);
                _fileStream.Position = currentPositionToRead + text.Length;
                _fileStream.Write(buffer, 0, numberOfBytesToRead);
                if (temp <= bufferSize)
                {
                    break;
                }
            }
            _fileStream.Position = from;
            _fileStream.Write(text, 0, text.Length);
        }

        public void Dispose()
        {
            _fileStream.Close();
        }
    }
}
