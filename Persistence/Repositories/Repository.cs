using Domain.Entities;
using Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    internal class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private DbSet<T> _entities;
        public virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();

                return _entities;
            }
        }

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> condition, CancellationToken cancellationToken = default)
            => await Entities.AnyAsync(condition, cancellationToken);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await Entities.AsNoTracking().ToListAsync(cancellationToken);
        
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>> predicate, CancellationToken cancellationToken = default)
            => await Entities.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);


        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await Entities.FindAsync(id, cancellationToken);

        public void Insert(T entity) => Entities.Add(entity);

        public void Remove(T entity) => Entities.Remove(entity);
    }
}
