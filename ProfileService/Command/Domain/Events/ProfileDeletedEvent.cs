using Infrastructure.Api.Base;

namespace ProfileService.Command.Domain.Events
{
    public class ProfileDeletedEvent : BaseEvent
    {
        public Guid Id { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string? Details { get; init; }

        public ProfileDeletedEvent(Guid id, string? details = null)
        {
            Id = id;
            Details = details;
        }
    }
}