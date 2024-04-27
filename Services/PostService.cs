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
            post.UserId = userId;
            post.PostAttachments = postCreateDto.PostAttachments.Select(p => new PostAttachment()
            {
                Name = p.File.Name,
                Url = p.File.FileName, // ToDo: Add service to file storage
                ContentType = p.File.ContentType,
            }).ToList();
            post.CreatedDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;
            post.CreatedBy = _loggedInUser.FindFirst(ClaimTypes.Email).Value;
            post.UpdatedBy = post.CreatedBy;

            _repositoryManager.PostRepo.Insert(post);
            
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return post.Adapt<PostDto>();
        }
    }
}
