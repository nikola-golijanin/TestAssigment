using System.ComponentModel.DataAnnotations;

namespace TestAssigmentAPI
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public record User(
            [Required] string FullName,
            [Required] string Username,
            [Required, EmailAddress] string Email,
            [Required] string PhoneNumber,
            [Required] string Language,
            [Required] string Culture,
            [Required] string Password);
}
