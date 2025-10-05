namespace Infrastructure.Api.Base;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public void Touch() => UpdatedAt = DateTime.UtcNow;
}
