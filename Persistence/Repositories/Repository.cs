using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class Repository<T> : IRepository<T> where T : class, IId
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
        

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)        
            => await Entities.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);        

        public void Insert(T entity) => Entities.Add(entity);    

        public void Remove(T entity) => Entities.Remove(entity);
    }
}
