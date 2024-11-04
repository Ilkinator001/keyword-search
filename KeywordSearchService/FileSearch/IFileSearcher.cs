﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearch
{
    public interface IFileSearcher
    {
        List<string> SearchFiles(string directory, string keyword);
    }
}
