using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using console.Domain;
using console.Extensions;

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

            // Convert event to Observable
            IObservable<EventPattern<ConsoleCancelEventArgs>> cancel =
                 Observable.FromEventPattern<ConsoleCancelEventHandler, ConsoleCancelEventArgs>(
                     h => Console.CancelKeyPress += h,
                     h => Console.CancelKeyPress -= h);
            cancel.SubscribeConsole();
            
            // IEnumerable to Observable 
            NumbersAndThrow()
                .ToObservable()
                .SubscribeConsole();
            
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        static IEnumerable<int> NumbersAndThrow()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            throw new ApplicationException("Something bad happened");
            yield return 4;
        }
    }
}
