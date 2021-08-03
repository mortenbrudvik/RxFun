using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            
            CreatingObservableSequences.Run();


            Console.WriteLine("Hello World!");
            //Console.ReadLine();
        }


    }
}
