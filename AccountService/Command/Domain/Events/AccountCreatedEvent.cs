using SharedKernel.Events;

namespace AccountService.Command.Domain.Events;

public class AccountCreatedEvent : BaseEvent
{
    public Guid AccountId { get; }
    public string Email { get; }

    public AccountCreatedEvent(Guid id, string email)
    {
        AccountId = id;
        Email = email;
    }
}
