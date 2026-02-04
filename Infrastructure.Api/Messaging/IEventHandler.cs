using System.Text.Json;

namespace Infrastructure.Api.Messaging;

/// <summary>
/// Generic event handler contract.
/// Implementations should expose the event type name they handle
/// and provide an async handler accepting the raw Json payload.
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// Fully qualified event type name used as the registration key.
    /// Example: "AccountService.Command.Domain.Events.AccountCreatedEvent"
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Handle the incoming event payload.
    /// </summary>
    Task HandleAsync(JsonElement payload);
}