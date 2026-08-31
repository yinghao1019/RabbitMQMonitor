
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQMonitor.Apis;
using RabbitMQMonitor.Configs;
using RabbitMQMonitor.Models;
using RabbitMQMonitor.Service;

namespace RabbitMQMonitor;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton<MailClient>();
        builder.Services.AddSingleton<RabbitMQClient>();
        builder.Services.AddSingleton<MailTemplateService>();
        builder.Services.AddSingleton<ConsumerMonitorService>();
        // fetch config
        builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<MailConfigs>(builder.Configuration.GetSection("MailConfig"));
        using var host = builder.Build();

        var monitorService = host.Services.GetRequiredService<ConsumerMonitorService>();
        await monitorService.ConsumerHealthCheck();
    }
}

