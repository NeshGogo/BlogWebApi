using Shared.Dtos;

namespace Services.Abstractions
{
    public interface IFollowService
    {
        Task<IEnumerable<UserFollowingDto>> GetUserFollowingAsync(CancellationToken cancellationToken = default);
        Task<UserFollowingDto> CreateFollowingUserAsync(Guid userToFollowId, CancellationToken cancellationToken = default);
    }
}
