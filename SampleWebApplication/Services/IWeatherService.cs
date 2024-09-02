namespace SampleWebApplication.Services;

public interface IWeatherService
{
    Task<WeatherForecast[]> GetWeatherForecast(int days = 5);
}