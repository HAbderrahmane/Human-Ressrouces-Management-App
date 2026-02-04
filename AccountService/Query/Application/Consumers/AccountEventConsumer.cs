using AccountService.Query.Domain;
using AccountService.Query.Infrastructure;
using Infrastructure.Api.Messaging;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AccountService.Query.Application.Consumers;

/// <summary>
/// Event handler for AccountCreatedEvent.
/// Implements IEventHandler so it can be discovered and registered automatically.
/// </summary>
public class AccountEventConsumer : IEventHandler
{
    private readonly ReadDbContext _readDb;
    private readonly ILogger<AccountEventConsumer> _logger;

    // Event type name used by the producer
    public string EventType => "AccountService.Command.Domain.Events.AccountCreatedEvent";

    public AccountEventConsumer(ReadDbContext readDb, ILogger<AccountEventConsumer> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task HandleAsync(JsonElement payload)
    {
        await HandleAccountCreatedAsync(payload);
    }

    public async Task HandleAccountCreatedAsync(JsonElement payload)
    {
        try
        {
            var accountId = Guid.Parse(payload.GetProperty("AccountId").GetString()!);
            var email = payload.GetProperty("Email").GetString()!;

            var readModel = new AccountReadModel
            {
                Id = accountId,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _readDb.Accounts.InsertOneAsync(readModel);
            _logger.LogInformation("Account read model created for {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AccountCreatedEvent");
        }
    }
}