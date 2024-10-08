﻿using Domain.Entities;
using Domain.Exceptions.FollowUser;
using Domain.Exceptions.User;
using Contracts;
using Mapster;
using Microsoft.AspNetCore.Http;
using Services.Abstractions;
using Shared.Dtos;
using System.Security.Claims;

namespace Services
{
    internal class FollowService : IFollowService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ClaimsPrincipal _loggedInUser;

        public FollowService(IHttpContextAccessor contextAccessor, IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _loggedInUser = contextAccessor.HttpContext.User;
        }

        public async Task<UserFollowingDto> CreateFollowingUserAsync(Guid userToFollowId, CancellationToken cancellationToken = default)
        {
            Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);
            if (userId.Equals(userToFollowId))
                throw new UserCannotFollowThemselvesException();

            var user = await _repositoryManager.UserRepo.GetByIdAsync(userToFollowId);

            if (user is null)
                throw new UserNotFoundException(userToFollowId);

            var exists = await _repositoryManager.UserFollowingRepo
                .ExistsAsync(p => p.UserId == userId && p.FollowingUserId == userToFollowId);

            if (exists)
                throw new UserAlreadyFollowException(userToFollowId);

            var userEmail = _loggedInUser.FindFirst(ClaimTypes.Email).Value;

            var userFollowing = new UserFollowing()
            {
                UserId = userId,
                FollowingUserId = userToFollowId,
                CreatedDate = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                CreatedBy = userEmail,
                UpdatedBy = userEmail,
            };

            _repositoryManager.UserFollowingRepo.Insert(userFollowing);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return userFollowing.Adapt<UserFollowingDto>();
        }

        public async Task DeleteFollowingUserAsync(Guid followUserId, bool isFollower = false, CancellationToken cancellationToken = default)
        {
            Guid.TryParse(_loggedInUser.FindFirst("Id").Value, out var userId);
            var followingUsers = isFollower 
                ? await _repositoryManager.UserFollowingRepo.GetAllAsync(p => p.UserId == followUserId
                        && p.FollowingUserId == userId, cancellationToken)
                : await _repositoryManager.UserFollowingRepo.GetAllAsync(p => p.UserId == userId 
                        && p.FollowingUserId == followUserId, cancellationToken);

            var user = followingUsers.FirstOrDefault();

            if (user is null)
                throw new UserNotFoundException(followUserId);

            _repositoryManager.UserFollowingRepo.Remove(user);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<UserFollowingDto>> GetUserFollowingAsync(Guid userId, bool following = true, CancellationToken cancellationToken = default)
        {
            var followings = following 
                ? await _repositoryManager.UserFollowingRepo.GetAllAsync(p => p.UserId == userId, cancellationToken)
                : await _repositoryManager.UserFollowingRepo.GetAllAsync(p => p.FollowingUserId == userId, cancellationToken);

            return followings.Select(p =>
                {
                    var result = p.Adapt<UserFollowingDto>();
                    result.FollowingUser = _repositoryManager.UserRepo.GetByIdAsync(p.FollowingUserId, cancellationToken)
                    .GetAwaiter().GetResult().Adapt<UserForBasicInfoDto>();
                    return result;
                });
        }
    }
}
