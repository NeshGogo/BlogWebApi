using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetAllByUserIdAsync(Guid UserId, CancellationToken cancellationToken = default);
    }
}
