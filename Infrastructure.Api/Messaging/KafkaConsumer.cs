using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Api.Messaging;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly string _topic;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer(string bootstrapServers, string topic, string groupId, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
        _topic = topic;

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer listening on '{Topic}'", _topic);
        _consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                if (result == null) continue;

                var wrapper = JsonSerializer.Deserialize<EventMessage>(result.Message.Value);
                if (wrapper == null)
                {
                    _logger.LogWarning("Invalid event format");
                    continue;
                }

                _logger.LogInformation("📦 Received event {EventType}", wrapper.EventType);

                await HandleEvent(wrapper.EventType, wrapper.Payload);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopping...");
        }
        finally
        {
            _consumer.Close();
        }
    }

    private Task HandleEvent(string eventType, JsonElement payload)
    {
        _logger.LogInformation("🔎 Dispatching event '{EventType}'", eventType);

        // TODO: Resolve handler from DI and call correct handler based on eventType

        return Task.CompletedTask;
    }
}

public class EventMessage
{
    public string EventType { get; set; }
    public JsonElement Payload { get; set; }
}
