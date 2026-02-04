using AccountService.Command.Domain;
using AccountService.Command.Domain.Events;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;

namespace AccountService.Command.Application.Handlers;

public class CreateAccountHandler : ICommandHandler<CreateAccountCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer _kafkaProducer;

    public CreateAccountHandler(IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer)
    {
        _unitOfWork = unitOfWork;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<Guid> HandleAsync(CreateAccountCommand request, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = request.Password,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Persister l'entité selon votre UnitOfWork/DbContext (exemple : ajout puis SaveChangesAsync)
        // Si vous n'avez pas ajouté l'entité au contexte, assurez-vous de le faire ici avant SaveChangesAsync.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accountCreatedEvent = new AccountCreatedEvent(account.Id, account.Email);
        var payload = new { AccountId = account.Id, Email = account.Email, CreatedAt = account.CreatedAt };

        // Publier sur le topic commun utilisé par le consumer
        await _kafkaProducer.ProduceAsync(accountCreatedEvent, payload, "account.events");

        return account.Id;
    }
}