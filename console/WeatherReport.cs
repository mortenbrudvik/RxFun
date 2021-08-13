namespace console
{
    public class WeatherReport
    {
        public double Temperature { get; set; }
        public string Station { get; set; }
        public override string ToString() => $"Station: {Station}, Temperature: {Temperature}";
    }
}