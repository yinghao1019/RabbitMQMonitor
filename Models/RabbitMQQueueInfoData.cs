using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQMonitor.Models
{
    /// <summary>
    /// One queue as returned by the RabbitMQ Management HTTP API
    /// (<c>GET /api/queues/{vhost}/{name}</c>, or an element of <c>GET /api/queues</c>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A deliberate subset of the response — only the fields this monitor consumes. The API returns
    /// far more (message_stats, garbage_collection, message_bytes_*, policies…); unmapped fields are
    /// ignored silently by <see cref="JsonSerializer"/>, so adding one back is just a matter of
    /// declaring the property.
    /// </para>
    /// <para>
    /// The API emits snake_case; the PascalCase names here are bound by the
    /// <c>SnakeCaseLower</c> naming policy held in <see cref="Apis.JsonUtils.Options"/>.
    /// Deserialize through <see cref="Apis.JsonUtils.Deserialize{T}(string)"/> — a bare
    /// <see cref="JsonSerializer"/> call with no options does not throw, it silently returns an
    /// instance with every property left at its default.
    /// </para>
    /// </remarks>
    public class RabbitMQQueueInfoData
    {
        /// <summary>Queue name — identifies which queue an alert is about.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Virtual host the queue lives in, e.g. <c>AMS</c>.</summary>
        public string Vhost { get; set; } = string.Empty;

        /// <summary>Number of consumers currently attached. Zero on a queue nobody is draining.</summary>
        public int Consumers { get; set; }

        /// <summary>Total depth: <see cref="MessagesReady"/> + <see cref="MessagesUnacknowledged"/>.</summary>
        public long Messages { get; set; }

        /// <summary>Messages waiting to be delivered — the usual backlog alarm signal.</summary>
        public long MessagesReady { get; set; }

        /// <summary>Delivered but not yet acked — a stuck consumer shows up here.</summary>
        public long MessagesUnacknowledged { get; set; }

        public List<RabbitMQConsumerDetail> ConsumerDetails { get; set; } = [];
    }

    /// <summary>One consumer attached to the queue.</summary>
    public class RabbitMQConsumerDetail
    {
        public string ConsumerTag { get; set; } = string.Empty;

        public RabbitMQChannelDetail? ChannelDetails { get; set; }

        public RabbitMQQueueReference? Queue { get; set; }

        public Dictionary<string, JsonElement> Arguments { get; set; } = [];

        public bool AckRequired { get; set; }

        public bool Active { get; set; }

        /// <summary>Usually <c>up</c>; <c>waiting</c> under single-active-consumer.</summary>
        public string ActivityStatus { get; set; } = string.Empty;

        /// <summary>Ack deadline in milliseconds before the channel is closed.</summary>
        public long ConsumerTimeout { get; set; }

        public bool Exclusive { get; set; }

        public int PrefetchCount { get; set; }
    }

    /// <summary>The channel (and its connection) a consumer is running on.</summary>
    public class RabbitMQChannelDetail
    {
        public string Name { get; set; } = string.Empty;

        public string ConnectionName { get; set; } = string.Empty;

        public string Node { get; set; } = string.Empty;

        public int Number { get; set; }

        public string PeerHost { get; set; } = string.Empty;

        public int PeerPort { get; set; }

        public string User { get; set; } = string.Empty;
    }

    /// <summary>Queue a consumer is bound to, as echoed inside <c>consumer_details</c>.</summary>
    public class RabbitMQQueueReference
    {
        public string Name { get; set; } = string.Empty;

        public string Vhost { get; set; } = string.Empty;
    }
}
