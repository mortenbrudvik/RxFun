using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using console.Domain.Temperature;
using console.Extensions;

namespace console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var temperatureService = new TemperatureService();
            var monitor = new TemperatureMonitor(temperatureService);
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.2});
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.2});
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.1});
            temperatureService.Notify(new Temperature(){City = "Bergen", Degrees = 10.3});
            temperatureService.Notify(new Temperature(){City = "Oslo", Degrees = 12.1});
            monitor.TemperatureChanges.SubscribeConsole("TemperatureMonitor");
            
            CreatingObservableSequences.Run();
            CreatingObservablesFromAsyncTypes.Run();
            ControllingObservableObserverLifetime.Run();
            ControllingTheObservableTemperature.Run();
            BasicQueries.Run();
            PartitioningAndCombining.Run();
            WorkingWithConcurrency.Run();
            ErrorHandlingAndRecovery.Run();
            
            // Disposable.create - Makes sure that isBusy is set to false even when an exception occur
            var isBusy = true;
            var newsItems = Enumerable.Empty<string>();
            using (Disposable.Create(() => isBusy = false)) 
                newsItems = await Task.FromResult(new[] { "news item 1", "news item 2" });
            newsItems.ToList().ForEach(Console.WriteLine);

            // Disposable.Empty - When you do not need any special disposing functionality
            Observable.Create<int>(o =>
            {
                o.OnNext(1);
                o.OnCompleted();
                return Disposable.Empty;
            }).SubscribeConsole("Disposable.Empty");

            Console.ReadLine();
        }
    }
}
