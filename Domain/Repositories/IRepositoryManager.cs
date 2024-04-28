using Domain.Entities;

namespace Domain.Repositories
{
    public interface IRepositoryManager
    {
        IRepository<User> UserRepo { get; }
        IPostRepository PostRepo { get; }
        ICommentRepository CommentRepo { get; }
        IUnitOfWork UnitOfWork { get; }
        IEmailRepository EmailRepository { get; }
    }
}
