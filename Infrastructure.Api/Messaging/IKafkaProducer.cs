namespace Infrastructure.Api.Messaging;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string message, string? key = null);
}