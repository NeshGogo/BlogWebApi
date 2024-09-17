using Domain.Entities;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllByPostIdAsync(Guid postId, CancellationToken cancellationToken = default)
            => await _context.Set<Comment>().AsNoTracking()
                .Include(p => p.User).Where(p => p.PostId == postId)
                .ToListAsync(cancellationToken);
    }
}
