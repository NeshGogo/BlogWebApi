using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllByPostIdAsync(Guid postId, CancellationToken cancellationToken = default);
    }
}
