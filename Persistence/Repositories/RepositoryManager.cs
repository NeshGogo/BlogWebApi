using Domain.Entities;
using Domain.Repositories;

namespace Persistence.Repositories
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IRepository<User>> _LazyUserRepo;
        private readonly Lazy<IRepository<Post>> _LazyPostRepo;
        private readonly Lazy<IRepository<Comment>> _LazyCommentRepo;
        private readonly Lazy<IUnitOfWork> _LazyUnitOfWork;

        public RepositoryManager(AppDbContext dbContext)
        {
            _LazyUserRepo = new Lazy<IRepository<User>>(() => new Repository<User>(dbContext));
            _LazyPostRepo = new Lazy<IRepository<Post>>(() => new Repository<Post>(dbContext));
            _LazyCommentRepo = new Lazy<IRepository<Comment>>(() => new Repository<Comment>(dbContext));
            _LazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
        }

        public IRepository<User> UserRepo => _LazyUserRepo.Value;

        public IRepository<Post> PostRepo => _LazyPostRepo.Value;

        public IRepository<Post> CommentRepo => _LazyPostRepo.Value;

        public IUnitOfWork UnitOfWork => _LazyUnitOfWork.Value;
    }
}
