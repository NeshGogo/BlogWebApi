using Domain.Entities;

namespace Contracts;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetAllByPostIdAsync(Guid postId, CancellationToken cancellationToken = default);
}
