using Domain.Entities;
using Domain.Repositories;
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
        }

        public async Task<PostDto> GetPostByAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.PostRepo.GetByIdAsync(postId, cancellationToken);
            
            if (post is null)
                throw new PostNotFoundException(postId);
           
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
        }
    }
}
