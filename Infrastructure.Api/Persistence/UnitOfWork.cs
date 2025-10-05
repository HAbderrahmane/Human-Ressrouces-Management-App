using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Api.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    public UnitOfWork(DbContext context) => _context = context;
    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
}
