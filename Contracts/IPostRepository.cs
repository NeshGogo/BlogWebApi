using Domain.Entities;

namespace Contracts;
public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetAllByUserIdAsync(Guid UserId, CancellationToken cancellationToken = default);
}
