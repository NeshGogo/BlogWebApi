using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos;
using Swashbuckle.AspNetCore.Annotations;


namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public FollowsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [SwaggerOperation(
            Summary = "Register a following user",
            Description = "You have to be log in.",
            Tags = ["Follows"]
            )]
        [HttpPost("Following"), Authorize]
        public async Task<ActionResult<UserFollowingDto>> AddFollowing([FromBody] UserFollowingForCreation createDto, CancellationToken cancellation)
            => await _serviceManager.followService.CreateFollowingUserAsync(createDto.FollowingUserId, cancellation);

    }
}
