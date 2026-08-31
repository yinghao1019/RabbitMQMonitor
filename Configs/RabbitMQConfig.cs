

namespace RabbitMQMonitor.Configs
{
    public class RabbitMQConfig
    {
        public string DomainUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public MonitorConfig Monitor { get; set; } = new();
    }

    public class MonitorConfig
    {
        public HealthThreshold HealthThreshold { get; set; } = new();

        /// <summary>Queues to poll. Named after the "Consumer" key in appsettings.json.</summary>
        public List<MonitorInfo> Consumer { get; set; } = [];
    }

    /// <summary>Alert when a queue falls outside any of these limits.</summary>
    public class HealthThreshold
    {
        /// <summary>Minimum attached consumers — fewer than this is an alert.</summary>
        public int Consumer { get; set; } = 1;

        /// <summary>Maximum tolerated backlog of ready (undelivered) messages.</summary>
        public long MessagesReady { get; set; } = 1000;

        /// <summary>Maximum tolerated delivered-but-unacked messages — a stuck consumer.</summary>
        public long Unacknowledged { get; set; } = 100;
    }

    public class MonitorInfo
    {
        public string QueueName { get; set; } = string.Empty;
        public string Vhost { get; set; } = string.Empty;
    }
}
