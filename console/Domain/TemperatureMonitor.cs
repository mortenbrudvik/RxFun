using System;
using System.Collections.Concurrent;

namespace console.Domain
{
    internal class TemperatureMonitor : IDisposable
    {
        private readonly TemperatureService _temperatureService;

        private ConcurrentDictionary<string, Temperature> _cityTemperatures = new ConcurrentDictionary<string, Temperature>();

        public TemperatureMonitor(TemperatureService temperatureService )
        {
            _temperatureService = temperatureService;
            _temperatureService.Changed += OnChanged;
        }

        private void OnChanged(object? sender, Temperature temperature)
        {
            if(_cityTemperatures.TryGetValue(temperature.City, out var oldTemp))
            {
                if (Math.Abs(temperature.Degrees - oldTemp.Degrees) > 0.001)
                {
                    Console.Out.WriteLine($"Temperature in {oldTemp.City} has changed from {oldTemp.Degrees} to {temperature.Degrees} ");
                    _cityTemperatures.TryUpdate(temperature.City, temperature, oldTemp);
                }
            }
            else
            {
                _cityTemperatures.TryAdd(temperature.City, temperature);
            }
        }

        public void Dispose()
        {
        }
    }
}