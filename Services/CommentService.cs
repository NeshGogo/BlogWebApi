using Domain.Entities;
using Domain.Exceptions.Post;
using Contracts;
using Mapster;
using Microsoft.AspNetCore.Http;
using Services.Abstractions;
using Shared.Dtos;
using System.Security.Claims;

namespace Services
{
    internal class CommentService : ICommentService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ClaimsPrincipal _loggedInUser;

        public CommentService(IHttpContextAccessor contextAccessor, IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _loggedInUser = contextAccessor.HttpContext.User;
        }

        public async Task<CommentDto> CreateComment(Guid postId, CommentForCreationDto creationDto, CancellationToken cancellation = default)
        {

            var postExits = await _repositoryManager.PostRepo.ExistsAsync(p => p.Id == postId, cancellation);
            
            if (!postExits)
                throw new PostNotFoundException(postId);

            var userEmail = _loggedInUser.FindFirst(ClaimTypes.Email).Value;
            Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);

            var comment = creationDto.Adapt<Comment>();            
            comment.PostId = postId;
            comment.CreatedDate = DateTime.UtcNow;
            comment.Updated = DateTime.UtcNow;
            comment.CreatedBy = userEmail;
            comment.UpdatedBy = userEmail;
            comment.UserId = userId;

            _repositoryManager.CommentRepo.Insert(comment);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellation);

            return comment.Adapt<CommentDto>();
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsByPost(Guid postId, CancellationToken cancellation = default)
        {
            var results = await _repositoryManager.CommentRepo.GetAllByPostIdAsync(postId, cancellation);
            return results.Adapt<IEnumerable<CommentDto>>().OrderByDescending(p => p.CreatedDate);
        }
    }
}
