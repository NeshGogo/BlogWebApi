using Domain.Entities;
using Domain.Storages;

namespace Contracts;

public interface IRepositoryManager
{
    IRepository<User> UserRepo { get; }
    IPostRepository PostRepo { get; }
    ICommentRepository CommentRepo { get; }
    IUnitOfWork UnitOfWork { get; }
    IEmailRepository EmailRepository { get; }
    IFileStorage FileStorage { get; }
    IRepository<PostLike> PostLikeRepo { get; }
    IRepository<UserFollowing> UserFollowingRepo { get; }
}
