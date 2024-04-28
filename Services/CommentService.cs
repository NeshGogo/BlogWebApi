using Domain.Repositories;
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

        public Task CreateComment(Guid postId, CommentForCreationDto creationDto, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsByPost(Guid postId, CancellationToken cancellation = default)
        {
            var results = await _repositoryManager.CommentRepo.GetAllByPostIdAsync(postId, cancellation);
            return results.Adapt<IEnumerable<CommentDto>>();
        }
    }
}
