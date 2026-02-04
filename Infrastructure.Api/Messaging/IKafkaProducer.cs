using SharedKernel.Events;

namespace Infrastructure.Api.Messaging;

public interface IKafkaProducer
{
    Task ProduceAsync(BaseEvent evt, object payload, string topic);
}