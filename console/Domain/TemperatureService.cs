using System;

namespace console.Domain
{
    internal class TemperatureService
    {
        public event EventHandler<Temperature> Changed;

        public void Notify(Temperature temperature)
        {
            Changed?.Invoke(this, temperature);
        }
    }
}