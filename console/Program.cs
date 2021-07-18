using System;
using console.Domain;

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
            
            
            Console.WriteLine("Hello World!");
        }
    }
}
