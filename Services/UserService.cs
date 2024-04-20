using Domain.Entities;
using Domain.Repositories;
using Domain.Exceptions.User;
using Mapster;
using Services.Abstractions;
using Shared;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Services
{
    internal sealed class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly UserManager<User> _userManager;

        public UserService(IRepositoryManager repositoryManager, UserManager<User> userManager)
        {
            _repositoryManager = repositoryManager;
            _userManager = userManager;
        }

        public async Task<UserDto> CreateAsync(UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default)
        {
            var user = userForCreationDto.Adapt<User>();            
            
            var exists = await _repositoryManager.UserRepo.ExistsAsync(p => p.Email == user.Email, cancellationToken);
            if (exists)
                throw new UserExistsByEmailException(user.Email);

            user.Updated = DateTime.UtcNow;
            user.CreatedDate = DateTime.UtcNow;
            user.CreatedBy = user.Email;
            user.UpdatedBy = user.Email;

            var result =  await _userManager.CreateAsync(user, userForCreationDto.Password);
            if (!result.Succeeded)
            {
                var msg = "There are some errors: \n";
                msg += string.Join("\n", result.Errors.Select(p => p.Description));
                throw new UserCreationErrorException(msg);
            }   

            return user.Adapt<UserDto>();
        }

        public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepo.GetByIdAsync(userId, cancellationToken);
            if(user is null)
                throw new UserNotFoundException(userId);

             _repositoryManager.UserRepo.Remove(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _repositoryManager.UserRepo.GetAllAsync(cancellationToken);
            var dtos = users.Adapt<IEnumerable<UserDto>>();
            return dtos;
        }

        public async Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepo.GetByIdAsync(userId, cancellationToken);
            var dto = user.Adapt<UserDto>();
            return dto;
        }

        public async Task UpdateAsync(Guid userId, UserForUpdateDto userForUpdateDto, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepo.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                throw new UserNotFoundException(userId);
            
            var userNameExists = await _repositoryManager.UserRepo.ExistsAsync(
                p => p.Id != userId && p.UserName == userForUpdateDto.UserName, 
                cancellationToken);
            if (userNameExists)
                throw new UserNameAlreadyExistsException(userForUpdateDto.UserName);

            user.Name = userForUpdateDto.Name;
            user.Bio = userForUpdateDto.Bio;
            user.UserName = userForUpdateDto.UserName;
            user.Updated = DateTime.UtcNow;

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
