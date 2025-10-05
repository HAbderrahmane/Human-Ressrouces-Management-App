namespace Infrastructure.Api.Messaging;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string key, string message);
}
