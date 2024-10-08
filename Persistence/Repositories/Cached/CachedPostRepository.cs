using Contracts;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Persistence.Repositories.Cached
{
    internal class CachedPostRepository : IPostRepository
    {

        private readonly IPostRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public CachedPostRepository(IPostRepository repository, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _repository = repository;
            _distributedCache = distributedCache;
        }

        public Task<bool> ExistsAsync(Expression<Func<Post, bool>> expression, CancellationToken cancellationToken = default)
            => _repository.ExistsAsync(expression, cancellationToken);

        public async Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var key = "Post-GetAll";
           
            string cachedPosts = await _distributedCache.GetStringAsync(key, cancellationToken);

            IEnumerable<Post> posts;
            
            if(string.IsNullOrEmpty(cachedPosts))
            {
                posts = await _repository.GetAllAsync(cancellationToken);
                
                if(!posts.Any()) 
                {
                    return posts;
                }

                await _distributedCache.SetStringAsync(
                    key,
                    JsonSerializer.Serialize(posts, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    }),
                    cancellationToken);
                
                return posts;
            }

            posts = JsonSerializer.Deserialize<IEnumerable<Post>>(cachedPosts);

            return posts;
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
