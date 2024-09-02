using Microsoft.Extensions.Caching.Memory;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SampleWebApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("OpenMeteo", opts => opts.BaseAddress = new Uri("https://api.open-meteo.com/"));
builder.Services.AddTransient<IWeatherService, OpenMeteoApiService>();
builder.Services.AddMemoryCache(opts =>
{
    opts.TrackStatistics = true;
    opts.ExpirationScanFrequency = TimeSpan.FromSeconds(5); // just for demo, this is way too low
});

builder.Logging.ClearProviders();
builder.Services.AddSingleton<Instrumentation>();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(builder => builder
        .AddService("SampleWebApplication", serviceVersion: "1.0.0")
        .AddTelemetrySdk())
    .WithLogging(builder =>
        builder
            .AddConsoleExporter()
            .AddOtlpExporter())
    .WithTracing(builder =>
        builder
            .AddConsoleExporter()
            .AddOtlpExporter()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(Instrumentation.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler()))
    .WithMetrics(builder =>
        builder
            .AddConsoleExporter()
            .AddOtlpExporter()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter(Instrumentation.MeterName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast",
        (IWeatherService weatherService) =>
        {
            var forecast = weatherService.GetWeatherForecast(5);

            return forecast;
        })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

var instrumentation = app.Services.GetRequiredService<Instrumentation>();
var cache = app.Services.GetRequiredService<IMemoryCache>();
instrumentation.MeasureCacheHits(() => cache.GetCurrentStatistics()!.TotalHits);
instrumentation.MeasureCacheMisses(() => cache.GetCurrentStatistics()!.TotalMisses);

app.Run();
