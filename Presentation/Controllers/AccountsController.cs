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
            Summary = "Create a new user",
            Description = "You don't required any permision to do it.",
            Tags = ["User settings"]
            )]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserForCreationDto creationDto, CancellationToken cancellation)
        {
            return await _serviceManager.UserService.CreateAsync(creationDto, cancellation);
        }
    }
}
