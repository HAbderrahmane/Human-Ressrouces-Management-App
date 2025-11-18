using MediatR;
using Infrastructure.Api.Messaging;
using AccountService.Command.Domain.Events;
using AccountService.Command.Application.Commands;

namespace AccountService.Command.Application.Handlers;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IKafkaProducer _producer;

    public CreateAccountHandler(IKafkaProducer producer)
    {
        _producer = producer;
    }

    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        var evt = new AccountCreatedEvent(id, request.Email);

        // ✅ NEW: send as event envelope, not string JSON
        await _producer.ProduceAsync(evt, evt, "account.events");

        Console.WriteLine($"AccountCreated event published for {request.Email}");

        return id;
    }
}
