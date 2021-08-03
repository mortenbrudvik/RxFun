using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using console.Domain;
using console.Extensions;
using console.SearchEngine;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            var temperatureService = new TemperatureService();
            var monitor = new TemperatureMonitor(temperatureService);
            
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.2});
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.2});
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.1});
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.3});
            temperatureService.Notify(new Temperature(){City = "Oslo", Degrees = 12.1});
            
            //CreatingObservableSequences.Run();

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
            
            
            

            Console.WriteLine("Hello World!");
            Console.ReadLine();
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
