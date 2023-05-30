using System.ComponentModel.DataAnnotations;

namespace odata_groupby
{
    public class WeatherForecast
    {
        [Key]
        public int Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}