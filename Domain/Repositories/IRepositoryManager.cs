using Domain.Entities;
using Domain.Storages;

namespace Domain.Repositories
{
    public interface IRepositoryManager
    {
        IRepository<User> UserRepo { get; }
        IPostRepository PostRepo { get; }
        ICommentRepository CommentRepo { get; }
        IUnitOfWork UnitOfWork { get; }
        IEmailRepository EmailRepository { get; }
        IFileStorage FileStorage { get; }
    }
}
