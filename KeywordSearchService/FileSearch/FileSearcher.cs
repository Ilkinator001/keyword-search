using System.Collections.Concurrent;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using NPOI.XWPF.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;
using NPOI.SS.UserModel;

namespace FileSearch
{
    public class FileSearcher : IFileSearcher
    {
        private static FileReader _reader = new FileReader();

        public List<string> SearchFiles(string directory, string keyword)
        {
            var results = new ConcurrentBag<string>();

            Parallel.ForEach(Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories),
                (file) =>
                {
                    if (FileContainsKeyword(file, keyword))
                    {
                        results.Add(file);
                    }
                });

            return results.ToList();
        }

        private bool FileContainsKeyword(string filePath, string keyword)
        {
            try
            {
                var content = _reader.GetContentFromFile(filePath);
                return content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false; // Fehlerbehandlung für Zugriffsprobleme
            }
        }
    }
}
