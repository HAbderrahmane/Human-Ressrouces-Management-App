using Confluent.Kafka;

namespace Infrastructure.Api.Messaging;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(ILogger<KafkaProducer> logger, string bootstrapServers)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers!,
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message, string? key = null)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = message
            });

            _logger.LogInformation("✅ Kafka message sent to {Topic} [{Partition}:{Offset}]", result.Topic, result.Partition, result.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "❌ Kafka produce error: {Reason}", ex.Error.Reason);
            throw;
        }
    }
}