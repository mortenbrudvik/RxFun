using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using console.Domain;
using console.Domain.Temperature;
using console.Extensions;

using static console.NumberGenerator;

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
            
            CreatingObservableSequences.Run();
            CreatingObservablesFromAsyncTypes.Run();
            ControllingObservableObserverLifetime.Run();
            ControllingTheObservableTemperature.Run();
            PartitioningAndCombining.Run();
            
            // Aggregate
            Observable.Range(1, 5)
                .Aggregate(1, (accumulate, currItem) => accumulate * currItem)
                .SubscribeConsole("Aggregate");
            
            // Scan
            Observable.Range(1, 5)
                .Scan(1, (accumulate, currItem) => accumulate * currItem)
                .SubscribeConsole("Scan");

            
            
            
            
            




            Console.ReadLine();
        }
    }
}
