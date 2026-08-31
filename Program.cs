
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQMonitor.Apis;
using RabbitMQMonitor.Configs;
using RabbitMQMonitor.Services;
using Serilog;

namespace RabbitMQMonitor;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        // Sinks and levels come from the "Serilog" section of appsettings.json.
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        // Drop the default console provider so nothing is written twice.
        builder.Logging.ClearProviders();
        builder.Services.AddSerilog(Log.Logger, dispose: true);

        builder.Services.AddSingleton<MailClient>();
        builder.Services.AddSingleton<RabbitMQClient>();
        builder.Services.AddSingleton<MailTemplateService>();
        builder.Services.AddSingleton<ConsumerMonitorService>();
        // fetch config
        builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<MailConfigs>(builder.Configuration.GetSection("MailConfig"));
        using var host = builder.Build();

        try
        {
            var monitorService = host.Services.GetRequiredService<ConsumerMonitorService>();
            await monitorService.ConsumerHealthCheck();
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            // Without this the process would die before the file sink flushes.
            Log.Fatal(ex, "Queue health check terminated unexpectedly.");
            Environment.Exit(1);
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}

