# otel-webinar-2024-samples

Samples used in the OpenTelemetry: The Road so Far webinar

This application is derived from the weather API application that gets scaffolded during a new project setup (`dotnet new webapi`).
The services are modified and enriched with caching and external API calls for the purposes of the demonstration. 

## Getting started with OpenTelemetry

See more about OpenTelemetry [here](https://opentelemetry.io).

See documentation on how to integrate OpenTelemetry in .NET [here](https://opentelemetry.io/docs/languages/net/).

See more about Aspire dashboard [here](https://learn.microsoft.com/sl-si/dotnet/aspire/fundamentals/dashboard/overview).

## Getting started with the demo web application

Install the following NuGet packages:

```
# Basic NuGet packages
dotnet add package OpenTelemetry
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
dotnet add package OpenTelemetry.Exporter.Console

# Additional instrumentation NuGet packages
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.Runtime
```

Instrument logs:

```csharp
builder.Services.AddOpenTelemetry()
    .WithLogging(builder =>
        builder.AddOtlpExporter());
```

Instrument traces:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
        builder
            .AddOtlpExporter()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .SetSampler(new AlwaysOnSampler()));
```

Instrument metrics:

```csharp
builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
        builder
            .AddOtlpExporter()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation());
```

## Getting started with Aspire dashboard

```bash
docker run --rm -it -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:8.1.0
```

