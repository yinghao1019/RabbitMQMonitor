using System.Text.Json;

namespace RabbitMQMonitor.Apis
{
    /// <summary>
    /// The single set of serializer settings for the RabbitMQ Management HTTP API, which emits snake_case.
    /// </summary>
    /// <remarks>
    /// The models in <c>Models/</c> deliberately carry no <c>[JsonPropertyName]</c> attributes, so they
    /// bind correctly <b>only</b> when these options are supplied. Prefer <see cref="Deserialize{T}"/> over
    /// calling <see cref="JsonSerializer"/> directly: omitting the options argument does not raise an error,
    /// it quietly produces an object with every property at its default — which a monitor would read as a
    /// healthy, empty queue.
    /// </remarks>
    public static class JsonUtils
    {
        /// <summary>Maps PascalCase properties onto the API's snake_case fields.</summary>
        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        /// <summary>Deserializes a RabbitMQ Management API response using <see cref="Options"/>.</summary>
        public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    }
}
