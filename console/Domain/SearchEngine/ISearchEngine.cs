using System.Collections.Generic;
using System.Threading.Tasks;

namespace console.Domain.SearchEngine
{
    public interface ISearchEngine
    {
        Task<IEnumerable<string>> Search(string term);
    }
}