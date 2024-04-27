using Domain.Entities;
using Domain.Exceptions.User;
using Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Abstractions;
using Shared.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    internal sealed class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal? _loggedInUser;
        private User? _user;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            IRepositoryManager repositoryManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _loggedInUser = _httpContextAccessor.HttpContext.User;
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
                var msg = "There are some errors: ";
                msg += string.Join(",", result.Errors.Select(p => p.Description));
                throw new UserCreationErrorException(msg);
            }

            // --> send confirm email notification
            var host = _configuration["HostUrl"];
            var emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
            var body = user.BuildConfirmEmailBody(host, emailConfirmToken);
            await _repositoryManager.EmailRepository.SendAsync([user.Email], "Email Confirmation", body, cancellation: cancellationToken);

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

        public async Task<TokenDto> LoginByEmailAndPassword(UserLoginDto userLogin, CancellationToken cancellationToken = default)
        {
            _user = await _userManager.FindByEmailAsync(userLogin.Email);

            if (_user is null)
                throw new UserNotFoundByEmailException(userLogin.Email);

            var result = await _signInManager.PasswordSignInAsync(_user, userLogin.Password, true, false);

            if (!result.Succeeded)
                throw new UserLoginByEmailOrPasswordException();

            var tokenDto = await CreateTokenAsync(true);
            return tokenDto;
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
            user.UpdatedBy = _loggedInUser.FindFirst(ClaimTypes.Email).Value;

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<TokenDto> CreateTokenAsync(bool populateExp)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;
            
            if(populateExp)
                _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(_user);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDto(token, refreshToken);
        }

        public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.Token);
            string userName = principal.FindFirst("UserName")?.Value;
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null || user.RefreshToken != tokenDto.RefreshToken || 
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new RefreshTokenException();

            _user = user;
            return await CreateTokenAsync(false);
        }

        public async Task ConfirmUserEmailAsync(string token, Guid userId, CancellationToken cancellation = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new UserNotFoundException(userId);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var msg = "There are some errors: ";
                msg += string.Join(",", result.Errors.Select(p => p.Description));
                throw new UserCreationErrorException(msg);
            }
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_configuration["jwt:key"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>()
            {
                new Claim("UserName", _user.UserName),
                new Claim(ClaimTypes.Email, _user.Email),
                new Claim(ClaimTypes.Name, _user.Name),
                new Claim("Id", _user.Id.ToString()),
            };

            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(_user);
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

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidator = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidIssuers = [_configuration["jwt:validIssuer"]],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(_configuration["jwt:key"])),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidator, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
