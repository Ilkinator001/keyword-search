using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordSearchService.Logic
{
    public class FileSearchService
    {
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
                var content = File.ReadAllText(filePath);
                return content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false; // Fehlerbehandlung für Zugriffsprobleme
            }
        }
    }
}
