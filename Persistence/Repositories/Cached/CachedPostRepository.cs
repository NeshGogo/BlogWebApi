using Contracts;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace Persistence.Repositories.Cached
{
    internal class CachedPostRepository : IPostRepository
    {

        private readonly IPostRepository _repository;
        private readonly IMemoryCache _memoryCache;

        public CachedPostRepository(IPostRepository repository, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _repository = repository;
        }

        public Task<bool> ExistsAsync(Expression<Func<Post, bool>> expression, CancellationToken cancellationToken = default)
            => _repository.ExistsAsync(expression, cancellationToken);

        public Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var key = "Post-GetAll";
            return _memoryCache.GetOrCreateAsync(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return _repository.GetAllAsync(cancellationToken);
            });
        }

        public Task<IEnumerable<Post>> GetAllAsync(Expression<Func<Post, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var key = $"Post-GetAllAsync-{predicate.Body}";
            return _memoryCache.GetOrCreateAsync(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return _repository.GetAllAsync(predicate, cancellationToken);
            });
        }

        public Task<IEnumerable<Post>> GetAllByUserIdAsync(Guid UserId, CancellationToken cancellationToken = default)
        {
            var key = $"Post-GetAllByUserId-{UserId}";
            return _memoryCache.GetOrCreateAsync(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return _repository.GetAllByUserIdAsync(UserId, cancellationToken);
            });  
        }

        public Task<Post> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var key = $"Post- GetById-{id}";
            return _memoryCache.GetOrCreateAsync(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return _repository.GetByIdAsync(id, cancellationToken);
            });
        }

        public void Insert(Post entity)
            => _repository.Insert(entity);

        public void Remove(Post entity)
            => _repository.Remove(entity);
    }
}
