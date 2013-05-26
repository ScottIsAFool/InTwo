using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;

namespace InTwo.Design
{
    public class DesignAsyncStorageService : IAsyncStorageService
    {
        public Task CopyFileAsync(string sourceFileName, string destinationFileName)
        {
            return new Task(null);
        }

        public Task CopyFileAsync(string sourceFileName, string destinationFileName, bool overwrite)
        {
            return new Task(null);
        }

        public Task CreateDirectoryAsync(string dir)
        {
            return new Task(null);
        }

        public Task<Stream> CreateFileAsync(string path)
        {
            return new Task<Stream>(null);
        }

        public Task DeleteDirectoryAsync(string dir)
        {
            return new Task(null);
        }

        public Task DeleteFileAsync(string path)
        {
            return new Task(null);
        }

        public Task<bool> DirectoryExistsAsync(string dir)
        {
            return new Task<bool>(null);
        }

        public Task<bool> FileExistsAsync(string path)
        {
            return new Task<bool>(null);
        }

        public Task<string[]> GetDirectoryNamesAsync()
        {
            return new Task<string[]>(null);
        }

        public Task<string[]> GetDirectoryNamesAsync(string searchPattern)
        {
            return new Task<string[]>(null);
        }

        public Task<string[]> GetFileNamesAsync()
        {
            return new Task<string[]>(null);
        }

        public Task<string[]> GetFileNamesAsync(string searchPattern)
        {
            return new Task<string[]>(null);
        }

        public Task<Stream> OpenFileForReadAsync(string path)
        {
            return new Task<Stream>(null);
        }

        public Task<string> ReadAllTextAsync(string path)
        {
            return new Task<string>(null);
        }

        public Task<string> ReadAllTextAsync(string path, Encoding encoding)
        {
            return new Task<string>(null);
        }

        public Task<string[]> ReadAllLinesAsync(string path)
        {
            return new Task<string[]>(null);
        }

        public Task<string[]> ReadAllLinesAsync(string path, Encoding encoding)
        {
            return new Task<string[]>(null);
        }

        public Task<byte[]> ReadAllBytesAsync(string path)
        {
            return new Task<byte[]>(null);
        }

        public Task WriteAllTextAsync(string path, string contents)
        {
            return new Task<string[]>(null);
        }

        public Task WriteAllTextAsync(string path, string contents, Encoding encoding)
        {
            return new Task<string[]>(null);
        }

        public Task WriteAllLinesAsync(string path, IEnumerable<string> contents)
        {
            return new Task<string[]>(null);
        }

        public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding)
        {
            return new Task<string[]>(null);
        }

        public Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            return new Task<string[]>(null);
        }

        public Task AppendAllText(string path, string contents)
        {
            return new Task<string[]>(null);
        }

        public Task AppendAllText(string path, string contents, Encoding encoding)
        {
            return new Task<string[]>(null);
        }

        public Task AppendAllLines(string path, IEnumerable<string> contents)
        {
            return new Task<string[]>(null);
        }

        public Task AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            return new Task<string[]>(null);
        }
    }
}