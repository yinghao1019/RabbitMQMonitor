
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQMonitor.Apis;
using RabbitMQMonitor.Configs;
using RabbitMQMonitor.Models;

namespace RabbitMQMonitor;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton<MailClient>();
        // fetch config
        builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<MailConfigs>(builder.Configuration.GetSection("MailConfig"));
        using var host = builder.Build();

        var service = host.Services.GetRequiredService<MailClient>();
        var mailConfig = host.Services.GetRequiredService<IOptions<MailConfigs>>().Value;
        var mailMessage = new EMailData
        {
            To = mailConfig.To,
            Subject = "test Mail",
            Body = "Test Console App",
        };

        await service.SendEmailAsync(mailMessage);
    }
}

