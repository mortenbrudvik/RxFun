using System;
using System.Reactive.Linq;

namespace console.Domain.Temperature
{
    internal class TemperatureMonitor : IDisposable
    {
        private readonly IDisposable _subscription;

        public TemperatureMonitor(TemperatureService temperatureService )
        {
            var temperatures = Observable.FromEventPattern<EventHandler<Temperature>, Temperature>(
                    h => temperatureService.Changed += h,
                    h => temperatureService.Changed -= h)
                .Select(changeEvent => changeEvent.EventArgs)
                .Synchronize();

            var temperatureChanges = from temperature in temperatures
                group temperature by temperature.City
                into city
                from temperaturePair in city.Buffer(2, 1)
                let temperatureDiff = Math.Abs(temperaturePair[0].Degrees - temperaturePair[1].Degrees)
                where temperatureDiff > 0.01
                select new TemperatureChange()
                {
                    City = city.Key,
                    DegreesOld = temperaturePair[0].Degrees,
                    DegreesNew = temperaturePair[1].Degrees
                };

            TemperatureChanges = temperatureChanges;

            _subscription = temperatureChanges.Subscribe(tempChange =>
            {
                Console.Out.WriteLine(
                    $"Temperature in {tempChange.City} has changed {tempChange.DegreesDifference} degrees from {tempChange.DegreesOld} to {tempChange.DegreesNew} ");
            });
        }

        public IObservable<TemperatureChange> TemperatureChanges { get; }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}