using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace console.SearchEngine
{
    public class SearchEngineA : ISearchEngine
    {
        public async Task<IEnumerable<string>> Search(string term)
        {
            await Console.Out.WriteLineAsync("Search Engine A");
            await Task.Delay(1500);
            return new[] { "Jupiter", "Mars" };
        }
    }
}