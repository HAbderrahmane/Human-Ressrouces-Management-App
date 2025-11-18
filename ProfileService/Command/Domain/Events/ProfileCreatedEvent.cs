using SharedKernel.Events;

namespace ProfileService.Command.Domain.Events
{
    public class ProfileCreatedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public ProfileCreatedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}
