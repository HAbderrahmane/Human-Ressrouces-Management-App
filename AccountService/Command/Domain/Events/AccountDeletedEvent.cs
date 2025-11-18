using SharedKernel.Events;

namespace AccountService.Command.Domain.Events
{
    public class AccountDeletedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public AccountDeletedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}
