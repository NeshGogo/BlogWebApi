
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Set<Post>().AsNoTracking()
                    .Include(p => p.PostAttachments.OrderBy(p=> p.CreatedDate))
                    .Include(p => p.User)
                    .Include(p => p.PostLikes)
                    .Include(p => p.Comments)
                    .ToListAsync(cancellationToken);
        public async Task<IEnumerable<Post>> GetAllByUserIdAsync(Guid UserId, CancellationToken cancellationToken = default)
            => await _context.Set<Post>().Include(p => p.PostAttachments)
                    .Where(p => p.UserId == UserId).ToListAsync(cancellationToken);

        public async Task<Post> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Set<Post>()
                    .Include(p => p.PostAttachments)
                    .Include(p => p.PostLikes)
                    .Include(p => p.Comments)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
