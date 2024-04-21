using Domain.Entities;

namespace Domain.Repositories
{
    public interface IRepositoryManager
    {
        IRepository<User> UserRepo { get; }
        IRepository<Post> PostRepo { get; }
        IRepository<Post> CommentRepo { get; }
        IUnitOfWork UnitOfWork { get; }
        IEmailRepository EmailRepository { get; }
    }
}
