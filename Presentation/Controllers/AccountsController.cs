using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Swashbuckle.AspNetCore.Annotations;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellation)
        {
            return (await _serviceManager.UserService.GetAllAsync(cancellation)).ToList();
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
        public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto, CancellationToken cancellation)
        {
            var token =  await _serviceManager.UserService.LoginByEmailAndPassword(userLoginDto, cancellation);
            return Ok(new
            {
                Token = token,
            });
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
