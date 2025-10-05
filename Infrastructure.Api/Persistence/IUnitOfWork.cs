namespace Infrastructure.Api.Persistence;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
}
