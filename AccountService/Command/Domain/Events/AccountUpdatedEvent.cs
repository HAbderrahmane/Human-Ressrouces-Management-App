using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events
{
    public class AccountUpdatedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public AccountUpdatedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}