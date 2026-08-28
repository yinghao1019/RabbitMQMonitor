

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQMonitor.Configs;
using RabbitMQMonitor.Extensions;
using RabbitMQMonitor.Models;

namespace RabbitMQMonitor.Apis
{
    public class MailClient
    {
        private readonly MailConfigs _mailConfig;
        private readonly ILogger<MailClient> _logger;
        public MailClient(IOptions<MailConfigs> mailConfig, ILogger<MailClient> logger)
        {
            _mailConfig = mailConfig.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EMailData eMailData)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("UUID", _mailConfig.Uuid);
            var requestUrl = _mailConfig.Url;
            _logger.LogInformation("Sending email via API: {requestUrl}", requestUrl);
            if (eMailData.To.FirstOrDefault<string>().IsBlank())
            {
                throw new Exception("Sending Email don't has receiver");
            }
            try
            {
                var json = JsonSerializer.Serialize(eMailData);
                _logger.LogDebug("Email data serialized to JSON: {json}", json);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully sent email to {To}", string.Join(",", eMailData.To));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send email. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                    throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while sending email.");
                throw;
            }
        }
    }
}