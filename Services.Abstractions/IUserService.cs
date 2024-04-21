using Shared.Dtos;

namespace Services.Abstractions
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<TokenDto> LoginByEmailAndPassword(UserLoginDto userLogin, CancellationToken cancellationToken = default);
        Task<TokenDto> CreateTokenAsync(bool populateExp);
        Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
        Task<UserDto> CreateAsync(UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid userId, UserForUpdateDto userForUpdateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
