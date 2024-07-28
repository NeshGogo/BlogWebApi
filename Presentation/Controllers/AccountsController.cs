using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.Metadata.Ecma335;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AccountsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [SwaggerOperation(
            Summary = "Get all users",
            Description = "You have to be log in.",
            Tags = ["Users"]
            )]
        [Authorize]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellation)
        {
            return (await _serviceManager.UserService.GetAllAsync(cancellation)).ToList();
        }

        [SwaggerOperation(
            Summary = "Get user",
            Description = "You have to be log in.",
            Tags = ["Users"]
            )]
        [Authorize]
        [HttpGet("users/{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken cancellation)
        {
            return await _serviceManager.UserService.GetByIdAsync(id, cancellation);
        }

        [SwaggerOperation(
            Summary = "Confirm user email after register",
            Description = "You have to be log in.",
            Tags = ["Users"]
            )]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string token, [FromQuery]Guid userId, CancellationToken cancellation)
        {
            await _serviceManager.UserService.ConfirmUserEmailAsync(token, userId, cancellation);
            return NoContent();
        }

        [SwaggerOperation(
            Summary = "Create a new user",
            Description = "You don't required any permision to do it.",
            Tags = ["Users"]
            )]
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserForCreationDto creationDto, CancellationToken cancellation)
        {
            return await _serviceManager.UserService.CreateAsync(creationDto, cancellation);
        }

        [SwaggerOperation(
            Summary = "Authenticate user by email and password",
            Description = "You don't required any permision to do it.",
            Tags = ["Users"]
            )]
        [HttpPost("Login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] UserLoginDto userLoginDto, CancellationToken cancellation)
        {
            return await _serviceManager.UserService.LoginByEmailAndPassword(userLoginDto, cancellation);
        }

        [SwaggerOperation(
            Summary = "Update user information",
            Description = "You have to be log in.",
            Tags = ["Users"]
            )]
        [Authorize]
        [HttpPut("Update/{id:Guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserForUpdateDto updateDto, CancellationToken cancellation)
        {
             await _serviceManager.UserService.UpdateAsync(id, updateDto, cancellation);
            return NoContent();
        }
    }
}
