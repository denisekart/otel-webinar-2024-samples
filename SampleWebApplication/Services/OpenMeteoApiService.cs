using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace SampleWebApplication.Services;

public class OpenMeteoApiService(
    IHttpClientFactory httpClientFactory, 
    ILogger<OpenMeteoApiService> logger,
    IMemoryCache cache) : IWeatherService
{
    string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    record OpenMeteoSummary(
        [property: JsonPropertyName("time")] List<DateOnly> Days,
        [property: JsonPropertyName("temperature_2m_max")]List<double> Temperatures);

    record OpenMeteoResponse(
        [property: JsonPropertyName("daily")] OpenMeteoSummary Summary);

    public async Task<WeatherForecast[]> GetWeatherForecast(int days = 5)
    {
        
        var client = httpClientFactory.CreateClient("OpenMeteo");

        var response = await cache.GetOrCreateAsync("weather",
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

                return await client.GetFromJsonAsync<OpenMeteoResponse>($"v1/forecast?latitude=46.05&longitude=14.5&daily=temperature_2m_max&timezone=auto&forecast_days={days}");
            });

        var forecasts = response.Summary.Days
            .Zip(response.Summary.Temperatures)
            .Select(x => new WeatherForecast(x.First,
                (int)x.Second,
                summaries[(int)TranslateRange(x.Second,
                    -40,
                    40,
                    0,
                    9)]))
            .ToArray();

        logger.LogInformation("Open Meteo API Request returned {Count} forecasts", forecasts.Length);
        return forecasts;
    }

    public static double TranslateRange(double value, double fromMin, double fromMax, double toMin, double toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}