using Contracts;
using Domain.Entities;
using Domain.Exceptions.Post;
using Mapster;
using Microsoft.AspNetCore.Http;
using Services.Abstractions;
using Shared.Dtos;
using System.Security.Claims;

namespace Services
{
    internal class PostService : IPostService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ClaimsPrincipal _loggedInUser;
        private readonly string container = "Posts";
        private readonly ICachingService _cachingService;

        public PostService(IHttpContextAccessor contextAccessor, IRepositoryManager repositoryManager, ICachingService cachingService)
        {
            _repositoryManager = repositoryManager;
            _loggedInUser = contextAccessor.HttpContext.User;
            _cachingService = cachingService;
        }

        public async Task AddOrRemovePostLikeAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.PostRepo.GetByIdAsync(postId, cancellationToken);

            if (post is null)
                throw new PostNotFoundException(postId);

            var userEmail = _loggedInUser.FindFirst(ClaimTypes.Email).Value;
            Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);

            var postLike = post.PostLikes.FirstOrDefault(p => p.UserId == userId);
            if (postLike is null)
            {
                var like = new PostLike
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    CreatedBy = userEmail,
                    UpdatedBy = userEmail,
                };
                _repositoryManager.PostLikeRepo.Insert(like);
            }
            else
            {
                _repositoryManager.PostLikeRepo.Remove(postLike);
            }

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            await _cachingService.RemoveByPrefixAsync("posts", cancellationToken);
        }

        public async Task<PostDto> CreatePostAsync(Guid userId, PostForCreationDto postCreateDto, CancellationToken cancellationToken = default)
        {
            var post = postCreateDto.Adapt<Post>();
            var userEmail = _loggedInUser.FindFirst(ClaimTypes.Email).Value;
            post.UserId = userId;

            post.PostAttachments = postCreateDto.Files.Select(file =>
            {
                var postAttch = new PostAttachment()
                {
                    Name = file.Name,
                    ContentType = file.ContentType,
                    CreatedDate = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    CreatedBy = userEmail,
                    UpdatedBy = userEmail,
                };
                using (var ms = new MemoryStream())
                {
                    file.CopyToAsync(ms);
                    var content = ms.ToArray();
                    var extension = Path.GetExtension(file.FileName);
                    postAttch.Url = _repositoryManager.FileStorage
                        .SaveFileAsync(content, extension, container, file.ContentType)
                        .GetAwaiter()
                        .GetResult();
                };

                return postAttch;
            }).ToList();
            post.CreatedDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;
            post.CreatedBy = userEmail;
            post.UpdatedBy = userEmail;

            _repositoryManager.PostRepo.Insert(post);

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            post.User = await _repositoryManager.UserRepo.GetByIdAsync(post.UserId);

            await _cachingService.RemoveByPrefixAsync("posts", cancellationToken);

            return post.Adapt<PostDto>();
        }

        public async Task DeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.PostRepo.GetByIdAsync(postId, cancellationToken);

            if (post is null)
                throw new PostNotFoundException(postId);

            Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);

            if (userId != post.UserId)
                throw new PostDoesNotBelongToUserException(userId);

            _repositoryManager.PostRepo.Remove(post);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            foreach (var attch in post.PostAttachments)
            {
                await _repositoryManager.FileStorage.RemoveFileAsync(container, attch.Url, cancellationToken);
            }

            await _cachingService.RemoveByPrefixAsync("posts", cancellationToken);
        }

        public async Task<PostDto> GetPostByAsync(Guid postId, CancellationToken cancellationToken = default) =>
            await _cachingService.GetAsync<PostDto>(
                $"Posts-{postId}",
                async () =>
                {
                    var post = await _repositoryManager.PostRepo.GetByIdAsync(postId, cancellationToken);

                    if (post is null)
                        throw new PostNotFoundException(postId);

                    Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);

                    var dto = post.Adapt<PostDto>();
                    dto.Liked = post.PostLikes.Any(x => x.UserId == userId);
                    dto.AmountOfComments = post.Comments.Count();
                    dto.AmountOfLikes = post.PostLikes.Count();

                    return dto;
                });

        public async Task<IEnumerable<PostDto>> GetPostsAllPost(bool following = false, CancellationToken cancellationToken = default)
        {
            Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);

            return await _cachingService.GetAsync<IEnumerable<PostDto>>(
                following ? $"Posts?following={following}&userId={userId}" : $"Posts?following={following}",
                async () =>
                {
                    IEnumerable<Post> posts;

                    if (following)
                    {
                        var users = await _repositoryManager.UserFollowingRepo.GetAllAsync(p => p.UserId == userId, cancellationToken);
                        var usersId = users.Select(p => p.FollowingUserId);
                        posts = await _repositoryManager.PostRepo.GetAllAsync(p => usersId.Any(p => p.Equals(userId)), cancellationToken);
                    }
                    else
                    {
                        posts = await _repositoryManager.PostRepo.GetAllAsync(cancellationToken);
                    }

                    return posts.Adapt<IEnumerable<PostDto>>().Select(p =>
                    {
                        var post = posts.First(x => x.Id == p.Id);
                        p.Liked = post.PostLikes.Any(x => x.UserId == userId);
                        p.AmountOfComments = post.Comments.Count();
                        p.AmountOfLikes = post.PostLikes.Count();
                        return p;
                    }).OrderByDescending(p => p.CreatedDate);
                },
                cancellationToken);
        }

        public async Task<IEnumerable<PostDto>> GetPostsByUserId(Guid userId, CancellationToken cancellationToken = default) =>
            await _cachingService.GetAsync<IEnumerable<PostDto>>(
                $"Posts-userId={userId}",
                async () =>
                {
                    var posts = await _repositoryManager.PostRepo.GetAllByUserIdAsync(userId, cancellationToken);
                    return posts.Adapt<IEnumerable<PostDto>>().OrderByDescending(p => p.CreatedDate);
                },
                cancellationToken);


        public async Task UpdatePostAsync(Guid postId, PostForUpdateDto updateDto, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.PostRepo.GetByIdAsync(postId, cancellationToken);

            if (post is null)
                throw new PostNotFoundException(postId);

            var userEmail = _loggedInUser.FindFirst(ClaimTypes.Email).Value;
            Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);

            if (userId != post.UserId)
                throw new PostDoesNotBelongToUserException(userId);

            post.Updated = DateTime.UtcNow;
            post.UpdatedBy = userEmail;
            post.Description = updateDto.Description;

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            await _cachingService.RemoveByPrefixAsync("Posts");
        }
    }
}
