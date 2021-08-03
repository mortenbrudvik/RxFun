using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace console.SearchEngine
{
    public interface ISearchEngine
    {
        Task<IEnumerable<string>> Search(string term);
    }
}