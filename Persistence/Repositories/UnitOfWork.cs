using Contracts;

namespace Persistence.Repositories
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext) => _dbContext = dbContext;
       
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
