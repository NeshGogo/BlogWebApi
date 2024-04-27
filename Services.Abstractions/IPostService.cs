using Shared.Dtos;


namespace Services.Abstractions
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(Guid userId, PostForCreationDto postCreateDto, CancellationToken cancellationToken = default);
    }
}
