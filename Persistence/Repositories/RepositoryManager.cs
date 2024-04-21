using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Persistence.Repositories
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IRepository<User>> _LazyUserRepo;
        private readonly Lazy<IRepository<Post>> _LazyPostRepo;
        private readonly Lazy<IRepository<Comment>> _LazyCommentRepo;
        private readonly Lazy<IUnitOfWork> _LazyUnitOfWork;
        private readonly Lazy<IEmailRepository> _LazyEmailRepo;

        public RepositoryManager(IConfiguration config, AppDbContext dbContext)
        {
            _LazyUserRepo = new Lazy<IRepository<User>>(() => new Repository<User>(dbContext));
            _LazyPostRepo = new Lazy<IRepository<Post>>(() => new Repository<Post>(dbContext));
            _LazyCommentRepo = new Lazy<IRepository<Comment>>(() => new Repository<Comment>(dbContext));
            _LazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
            _LazyEmailRepo = new Lazy<IEmailRepository>(() => new EmailRepository(config));
        }

        public IRepository<User> UserRepo => _LazyUserRepo.Value;

        public IRepository<Post> PostRepo => _LazyPostRepo.Value;

        public IRepository<Post> CommentRepo => _LazyPostRepo.Value;

        public IUnitOfWork UnitOfWork => _LazyUnitOfWork.Value;
        public IEmailRepository EmailRepository => _LazyEmailRepo.Value;
    }
}
