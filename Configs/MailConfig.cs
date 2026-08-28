

namespace RabbitMQMonitor.Configs
{
    public class MailConfigs
    {
        public string Url { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public List<string> To { get; set; } = [];
    }
}