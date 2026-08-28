using Microsoft.Extensions.Hosting;

namespace RabbitMQMonitor;

class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        using var host = builder.Build();
    }
}
