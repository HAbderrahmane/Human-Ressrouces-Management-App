using Infrastructure.Api.Base;

namespace AccountService.Command.Domain.Events
{
    public class AccountRoleUpdatedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public AccountRoleUpdatedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}