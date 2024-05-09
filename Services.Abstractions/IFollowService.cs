using Shared.Dtos;

namespace Services.Abstractions
{
    public interface IFollowService
    {
        Task<UserFollowingDto> CreateFollowingUserAsync(Guid userToFollowId, CancellationToken cancellationToken = default);
    }
}
