using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using console.Domain.SearchEngine;
using console.Extensions;

namespace console
{
    public static class CreatingObservablesFromAsyncTypes
    {
        public static void Run()
        {
            // Using Task.Run in observable creation
            NumberGenerator
                .GeneratePrimes(5)
                .Timestamp()
                .SubscribeConsole();
            
            // Using  async-await in observable creation
            Search("ss")
                .SubscribeConsole();
            
            // Converting tasks to observables
            Search2("sd")
                .SubscribeConsole();
            
            // Running async code as part of the pipeline
            Observable.Range(1, 10)
                .SelectMany(number => number.IsPrimeNumber(),
                    (number, isPrime) => new { number, isPrime })
                .Where(x => x.isPrime)
                .Select(x => x.number)
                .SubscribeConsole();
            
            // Make sure that the primes is sorted
            Observable.Range(1, 10)
                .Select(async number => new { number, isPrime = await number.IsPrimeNumber() })
                .Concat()
                .Where(x => x.isPrime)
                .Select(x => x.number)
                .SubscribeConsole();
            
            // Emitting values in time intervals
            var counter = 0;
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(x => counter++)
                .Take(3)
                .SubscribeConsole("Interval");
            
            // Scheduling an emission with a timer
            Observable.Timer(TimeSpan.FromSeconds(1))
                .Select(x => counter++)
                .Take(3)
                .SubscribeConsole("Timer");
        }
        private static IObservable<string> Search(string term)
        {
            return Observable.Create<string>(async o =>
            {
                var engineA = new SearchEngineA();
                var engineB = new SearchEngineB();

                var resultA = await engineA.Search(term);
                foreach (var result in resultA)
                    o.OnNext(result);
                var resultB = await engineB.Search(term);
                foreach (var result in resultB)
                    o.OnNext(result);

                o.OnCompleted();
            });

        }

        private static IObservable<string> Search2(string term)
        {
            var engineA = new SearchEngineA();
            var engineB = new SearchEngineB();
            var resultA = engineA.Search(term).ToObservable();
            var resultB = engineB.Search(term).ToObservable();

            return resultA
                .Concat(resultB)
                .SelectMany(results => results);
        }
    }
}