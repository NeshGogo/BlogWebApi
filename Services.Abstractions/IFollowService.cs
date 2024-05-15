using Shared.Dtos;

namespace Services.Abstractions
{
    public interface IFollowService
    {
        Task<IEnumerable<UserFollowingDto>> GetUserFollowingAsync(bool following = true, CancellationToken cancellationToken = default);
        Task<UserFollowingDto> CreateFollowingUserAsync(Guid userToFollowId, CancellationToken cancellationToken = default);
        Task DeleteFollowingUserAsync(Guid userId, bool isFollower = false, CancellationToken cancellationToken = default);
    }
}
