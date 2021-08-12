using System.Reactive.Linq;
using console.Extensions;

namespace console
{
    public static class BasicQueries
    {
        public static void Run()
        {
            // Aggregate
            Observable.Range(1, 5)
                .Aggregate(1, (accumulate, currItem) => accumulate * currItem)
                .SubscribeConsole("Aggregate");
            
            // Scan
            Observable.Range(1, 5)
                .Scan(1, (accumulate, currItem) => accumulate * currItem)
                .SubscribeConsole("Scan");

        }
    }
}