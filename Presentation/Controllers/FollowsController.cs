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
            Summary = "Get a following by user",
            Description = "You have to be log in.",
            Tags = ["Follows"]
            )]
        [HttpGet("Following"), Authorize]
        public async Task<ActionResult<IEnumerable<UserFollowingDto>>> GetFollowing([FromQuery] bool following = true, 
                CancellationToken cancellation = default)
            => (await _serviceManager.followService.GetUserFollowingAsync(following, cancellation)).ToList();

        [SwaggerOperation(
            Summary = "Register a following user",
            Description = "You have to be log in.",
            Tags = ["Follows"]
            )]
        [HttpPost("Following"), Authorize]
        public async Task<ActionResult<UserFollowingDto>> AddFollowing([FromBody] UserFollowingForCreation createDto, CancellationToken cancellation)
            => await _serviceManager.followService.CreateFollowingUserAsync(createDto.FollowingUserId, cancellation);

        [SwaggerOperation(
           Summary = "Delete a following user",
           Description = "You have to be log in.",
           Tags = ["Follows"]
           )]
        [HttpDelete("Unfllow/{followUserId:guid}"), Authorize]
        public async Task<IActionResult> RemoveFollowing(Guid followUserId, CancellationToken cancellation)
        { 
            await _serviceManager.followService.DeleteFollowingUserAsync(followUserId, cancellation);
            return NoContent();
        }
    }
}
