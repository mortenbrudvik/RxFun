using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace console.SearchEngine
{
    public class SearchEngineB : ISearchEngine
    {
        public async Task<IEnumerable<string>> Search(string term)
        {
            await Console.Out.WriteLineAsync("Search Engine B");
            await Task.Delay(1500);
            return new[] { "Earth", "Venus" };
        }
    }
}