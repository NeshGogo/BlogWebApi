using Domain.Entities;
using Domain.Repositories;
using Domain.Exceptions.User;
using Mapster;
using Services.Abstractions;
using Shared;

namespace Services
{
    internal sealed class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<UserDto> CreateAsync(UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default)
        {
            var user = userForCreationDto.Adapt<User>();
            var exists = await _repositoryManager.UserRepo.ExistsAsync(p => p.Email == user.Email, cancellationToken);
            if (exists)
                throw new UserExistsByEmailException(user.Email);
            _repositoryManager.UserRepo.Insert(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
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
           
            user.Name = userForUpdateDto.Name;
            user.Bio = userForUpdateDto.Bio;
            user.UserName = userForUpdateDto.UserName;

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
