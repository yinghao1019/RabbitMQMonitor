

using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQMonitor.Configs;
using RabbitMQMonitor.Extensions;
using RabbitMQMonitor.Models;

namespace RabbitMQMonitor.Apis
{
    public class RabbitMQClient
    {
        private readonly RabbitMQConfig _rabbitmqConfig;
        private readonly ILogger<RabbitMQClient> _logger;
        public RabbitMQClient(IOptions<RabbitMQConfig> rabbitMqConfig, ILogger<RabbitMQClient> logger)
        {
            _rabbitmqConfig = rabbitMqConfig.Value;
            _logger = logger;
        }

        public async Task<RabbitMQQueueInfoData> GetQueueDetailInfo(string vHost, string queueName)
        {
            var httpClient = new HttpClient();
            // set auth
            var credential = GetClientBasicAuth(_rabbitmqConfig.UserName, _rabbitmqConfig.Password);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
            // A blank vHost means the default virtual host, which the Management API names "/" —
            // and a path segment of "/" has to be percent-encoded as %2F or the URL collapses to a 404.
            var vHostSegment = Uri.EscapeDataString(vHost.IsBlank() ? "/" : vHost);
            var requestUrl = $"{_rabbitmqConfig.DomainUrl}/api/queues/{vHostSegment}/{queueName}";
            _logger.LogInformation("Querying queue detail via API: {requestUrl}", requestUrl);
            try
            {
                var response = await httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to query queue detail. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                    throw new Exception($"Failed to query queue detail. Status Code: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Queue detail response: {json}", json);

                var queueInfo = JsonUtils.Deserialize<RabbitMQQueueInfoData>(json)
                    ?? throw new Exception($"Queue detail response for {vHost}/{queueName} deserialized to null.");
                return queueInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while querying queue detail of {vHost}/{queueName}.", vHost, queueName);
                throw;
            }
        }

        public string GetClientBasicAuth(string userName, string password)
        {
            string credential = $"{userName}:{password}";
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));
            return base64;
        }
    }
}