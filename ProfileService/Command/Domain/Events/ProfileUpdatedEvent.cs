using SharedKernel.Events;

namespace ProfileService.Command.Domain.Events
{
    public class ProfileUpdatedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public ProfileUpdatedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}