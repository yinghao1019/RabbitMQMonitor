

namespace RabbitMQMonitor.Models
{
    /// <summary>
    /// The values a queue alert template renders. Bound by Scriban's standard member renamer,
    /// so <c>QueueName</c> is written <c>{{ queue_name }}</c> in the HTML under <c>Templates/</c>.
    /// </summary>
    public class QueueAlertData
    {
        /// <summary>Mail subject, also used as the template's &lt;title&gt;.</summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>Management UI address operators should open, from <c>RabbitMQ:DomainUrl</c>.</summary>
        public string DomainUrl { get; set; } = string.Empty;

        public string Vhost { get; set; } = string.Empty;

        public string QueueName { get; set; } = string.Empty;

        public int Consumers { get; set; }

        public long Messages { get; set; }

        public long MessagesReady { get; set; }

        public long MessagesUnacknowledged { get; set; }

        /// <summary>Configured limits, shown next to the measured values.</summary>
        public int ConsumerThreshold { get; set; }

        public long MessagesReadyThreshold { get; set; }

        public long UnacknowledgedThreshold { get; set; }

        public string CheckedAt { get; set; } = string.Empty;
    }
}
