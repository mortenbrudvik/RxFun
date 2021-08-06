namespace console.Domain.Temperature
{
    public class TemperatureChange
    {
        public string City { get; set; }
        public double DegreesNew { get; set; }
        public double DegreesOld { get; set; }
        public double DegreesDifference { get; set; }
    }
}