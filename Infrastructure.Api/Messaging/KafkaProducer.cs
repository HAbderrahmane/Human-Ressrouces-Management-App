using Confluent.Kafka;
using Confluent.Kafka.Admin;
using SharedKernel.Events;
using System.Text.Json;

namespace Infrastructure.Api.Messaging;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly string _bootstrapServers;

    public KafkaProducer(ILogger<KafkaProducer> logger, string bootstrapServers)
    {
        _logger = logger;
        _bootstrapServers = bootstrapServers;

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    private async Task EnsureTopicExistsAsync(string topic)
    {
        using var admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _bootstrapServers
        }).Build();

        try
        {
            var metadata = admin.GetMetadata(TimeSpan.FromSeconds(5));
            bool exists = metadata.Topics.Any(t => t.Topic == topic);

            if (!exists)
            {
                await admin.CreateTopicsAsync(new[]
                {
                    new TopicSpecification
                    {
                        Name = topic,
                        NumPartitions = 1,
                        ReplicationFactor = 1
                    }
                });
            }
        }
        catch { }
    }

    public async Task ProduceAsync(BaseEvent evt, object payload, string topic)
    {
        var envelope = EventEnvelope.FromEvent(evt, payload);
        var json = JsonSerializer.Serialize(envelope);

        await EnsureTopicExistsAsync(topic);

        await _producer.ProduceAsync(
            topic,
            new Message<string, string>
            {
                Key = evt.EventId.ToString(),
                Value = json
            }
        );
    }
}
