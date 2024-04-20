using Domain.Entities;
using Domain.Exceptions.User;
using Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Abstractions;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    internal sealed class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public UserService(
            IRepositoryManager repositoryManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

            var result = await _userManager.CreateAsync(user, userForCreationDto.Password);
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
            if (user is null)
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

        public async Task<string> LoginByEmailAndPassword(UserLoginDto userLogin, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(userLogin.Email);

            if (user is null)
                throw new UserNotFoundByEmailException(userLogin.Email);

            _userManager.CheckPasswordAsync(user, userLogin.Password);
            var result = await _signInManager.PasswordSignInAsync(user, userLogin.Password, true, false);

            if (!result.Succeeded)
                throw new UserLoginByEmailOrPasswordException();

            var token = await CreateTokenAsync(user);
            return token;
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


        public async Task<string> CreateTokenAsync(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_configuration["jwt:key"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("UserName", user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
            };

            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            
            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("jwt");
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }
    }
}
