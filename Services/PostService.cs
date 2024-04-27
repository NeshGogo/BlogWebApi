using Domain.Entities;
using Domain.Repositories;
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

        public PostService(IHttpContextAccessor contextAccessor, IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _loggedInUser = contextAccessor.HttpContext.User;
        }

        public async Task<PostDto> CreatePostAsync(Guid userId, PostForCreationDto postCreateDto, CancellationToken cancellationToken = default)
        {
            var post = postCreateDto.Adapt<Post>();
            var userEmail = _loggedInUser.FindFirst(ClaimTypes.Email).Value;
            post.UserId = userId;
            post.PostAttachments = postCreateDto.Files.Select(file => new PostAttachment()
            {
                Name = file.Name,
                Url = file.FileName, // ToDo: Add service to file storage
                ContentType = file.ContentType,
                CreatedDate = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                CreatedBy = userEmail,
                UpdatedBy = userEmail,
            }).ToList();
            post.CreatedDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;
            post.CreatedBy = userEmail;
            post.UpdatedBy = userEmail;

            _repositoryManager.PostRepo.Insert(post);

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return post.Adapt<PostDto>();
        }

        public async Task<PostDto> GetPostByAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.PostRepo.GetByIdAsync(postId, cancellationToken);
            return post.Adapt<PostDto>();
        }

        public async Task<IEnumerable<PostDto>> GetPostsAllPost(CancellationToken cancellationToken = default)
        {
            var posts = await _repositoryManager.PostRepo.GetAllAsync(cancellationToken);
            return posts.Adapt<IEnumerable<PostDto>>();
        }

        public async Task<IEnumerable<PostDto>> GetPostsByUserId(Guid userId, CancellationToken cancellationToken = default)
        {
            var posts = await _repositoryManager.PostRepo.GetAllByUserIdAsync(userId, cancellationToken);
            return posts.Adapt<IEnumerable<PostDto>>();
        }
    }
}
