using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Persistence.Repositories
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IRepository<User>> _LazyUserRepo;
        private readonly Lazy<IPostRepository> _LazyPostRepo;
        private readonly Lazy<ICommentRepository> _LazyCommentRepo;
        private readonly Lazy<IUnitOfWork> _LazyUnitOfWork;
        private readonly Lazy<IEmailRepository> _LazyEmailRepo;

        public RepositoryManager(IConfiguration config, AppDbContext dbContext)
        {
            _LazyUserRepo = new Lazy<IRepository<User>>(() => new Repository<User>(dbContext));
            _LazyPostRepo = new Lazy<IPostRepository>(() => new PostRepository(dbContext));
            _LazyCommentRepo = new Lazy<ICommentRepository>(() => new CommentRepository(dbContext));
            _LazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
            _LazyEmailRepo = new Lazy<IEmailRepository>(() => new EmailRepository(config));
        }

        public IRepository<User> UserRepo => _LazyUserRepo.Value;

        public IPostRepository PostRepo => _LazyPostRepo.Value;

        public ICommentRepository CommentRepo => _LazyCommentRepo.Value;

        public IUnitOfWork UnitOfWork => _LazyUnitOfWork.Value;
        public IEmailRepository EmailRepository => _LazyEmailRepo.Value;
    }
}
