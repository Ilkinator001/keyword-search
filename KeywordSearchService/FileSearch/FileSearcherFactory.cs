using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearch
{
    public class FileSearcherFactory : IFileSearcherFactory
    {
        public IFileSearcher CreateFileSearcher()
        {
            return new FileSearcher();
        }
    }
}
