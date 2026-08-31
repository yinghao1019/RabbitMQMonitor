

using Microsoft.Extensions.Options;
using RabbitMQMonitor.Apis;
using RabbitMQMonitor.Configs;
using RabbitMQMonitor.Extensions;
using RabbitMQMonitor.Models;

namespace RabbitMQMonitor.Services
{
    public class ConsumerMonitorService
    {
        public const string ConsumerDownAlert = "ConsumerDownAlert.html";
        public const string QueueBacklogAlert = "QueueBacklogAlert.html";
        private readonly RabbitMQConfig _rabbitmqConfig;
        private readonly MailConfigs _mailConfig;
        private readonly MailClient _mailClient;
        private readonly RabbitMQClient _rabbitmqClient;
        private readonly MailTemplateService _mailTemplateService;
        public ConsumerMonitorService(IOptions<RabbitMQConfig> rabbitMQConfig, IOptions<MailConfigs> mailConfig, MailClient mailClient, RabbitMQClient rabbitMQClient, MailTemplateService mailTemplateService)
        {
            _rabbitmqConfig = rabbitMQConfig.Value;
            _mailConfig = mailConfig.Value;
            _mailClient = mailClient;
            _rabbitmqClient = rabbitMQClient;
            _mailTemplateService = mailTemplateService;
        }

        public async Task ConsumerHealthCheck()
        {
            var queueConsumer = _rabbitmqConfig.Monitor.Consumer;
            var healthThreshold = _rabbitmqConfig.Monitor.HealthThreshold;
            foreach (var consumer in queueConsumer)
            {
                if (consumer.QueueName.IsBlank())
                {
                    continue;
                }
                var queueInfo = await _rabbitmqClient.GetQueueDetailInfo(consumer.Vhost, consumer.QueueName);
                if (queueInfo.Consumers < healthThreshold.Consumer)
                {
                    // send Mail Notification Alert
                    var subject = $"[Critical] Queue Consumer 停止運作 - {queueInfo.Vhost}/{queueInfo.Name}";
                    await SendAlertMailAsync(ConsumerDownAlert, subject, queueInfo, healthThreshold);
                }
                else if (queueInfo.MessagesReady > healthThreshold.MessagesReady || queueInfo.MessagesUnacknowledged > healthThreshold.Unacknowledged)
                {
                    // send Mail Notification Warning
                    var subject = $"[Warning] Queue Message 過多,消化不了 - {queueInfo.Vhost}/{queueInfo.Name}";
                    await SendAlertMailAsync(QueueBacklogAlert, subject, queueInfo, healthThreshold);
                }
            }
        }

        private async Task SendAlertMailAsync(string templateName, string subject, RabbitMQQueueInfoData queueInfo, HealthThreshold healthThreshold)
        {
            var alertData = new QueueAlertData
            {
                Subject = subject,
                DomainUrl = _rabbitmqConfig.DomainUrl,
                // A blank vhost is the default one, which the Management API names "/".
                Vhost = queueInfo.Vhost.IsBlank() ? "/" : queueInfo.Vhost,
                QueueName = queueInfo.Name,
                Consumers = queueInfo.Consumers,
                Messages = queueInfo.Messages,
                MessagesReady = queueInfo.MessagesReady,
                MessagesUnacknowledged = queueInfo.MessagesUnacknowledged,
                ConsumerThreshold = healthThreshold.Consumer,
                MessagesReadyThreshold = healthThreshold.MessagesReady,
                UnacknowledgedThreshold = healthThreshold.Unacknowledged,
                CheckedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            var body = await _mailTemplateService.RenderAsync(templateName, alertData);
            await _mailClient.SendEmailAsync(new EMailData
            {
                To = _mailConfig.To,
                Subject = subject,
                Body = body,
                IsHtml = true,
                IsImportant = true,
            });
        }
    }
}
