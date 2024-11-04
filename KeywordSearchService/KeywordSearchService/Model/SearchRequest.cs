using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordSearchService.Model
{
    public class SearchRequest
    {
        public string Directory { get; set; }
        public string Keyword { get; set; }
    }
}
