using Shared.Dtos;


namespace Services.Abstractions
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(Guid userId, PostForCreationDto postCreateDto, CancellationToken cancellationToken = default);
        Task<PostDto> GetPostByAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostDto>> GetPostsByUserId(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostDto>> GetPostsAllPost(bool following = false, CancellationToken cancellationToken = default);
        Task UpdatePostAsync(Guid postId, PostForUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task AddOrRemovePostLikeAsync(Guid postId, CancellationToken cancellationToken = default);
        Task DeletePostAsync(Guid postId, CancellationToken cancellationToken = default);
    }
}
