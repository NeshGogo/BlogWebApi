
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal class PostRepository : Repository<Post>, IRepository<Post>
    {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Post> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Set<Post>().Include(p => p.PostAttachments)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
