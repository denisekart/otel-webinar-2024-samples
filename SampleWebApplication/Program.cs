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

app.Run();
