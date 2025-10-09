using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Api.Messaging;

public class KafkaConsumer : BackgroundService, IKafkaConsumer
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
        _logger.LogInformation("Kafka consumer listening on topic '{Topic}'", _topic);
        _consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    if (result is null) continue;

                    _logger.LogInformation("Message received: {Message}", result.Message.Value);

                    // TODO: process your message here
                    await HandleMessageAsync(result.Message.Key, result.Message.Value);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopping...");
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }

    private Task HandleMessageAsync(string key, string value)
    {
        // Here you could deserialize JSON and handle specific events
        _logger.LogDebug("Handling Kafka message Key={Key} Value={Value}", key, value);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka consumer stopped.");
        return base.StopAsync(cancellationToken);
    }
}