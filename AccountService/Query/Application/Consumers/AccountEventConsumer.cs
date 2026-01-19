using System.Text.Json;
using AccountService.Command.Domain.Events;
using AccountService.Query.Domain;
using AccountService.Query.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AccountService.Query.Application.Consumers;

public class AccountEventConsumer
{
    private readonly ReadDbContext _readDb;
    private readonly ILogger<AccountEventConsumer> _logger;

    public AccountEventConsumer(ReadDbContext readDb, ILogger<AccountEventConsumer> logger)
    {
        _readDb = readDb;
        _logger = logger;
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
            _logger.LogInformation("✅ Account read model created for {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling AccountCreatedEvent");
            // Don't rethrow - let the consumer continue
        }
    }
}
