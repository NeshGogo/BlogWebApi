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
        [HttpGet("Following/{userId:guid}"), Authorize]
        public async Task<ActionResult<IEnumerable<UserFollowingDto>>> GetFollowing(Guid userId, [FromQuery] bool following = true, 
                CancellationToken cancellation = default)
            => (await _serviceManager.followService.GetUserFollowingAsync(userId, following, cancellation)).ToList();

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
        [HttpDelete("Unfollow/{followUserId:guid}"), Authorize]
        public async Task<IActionResult> RemoveFollowing(Guid followUserId, CancellationToken cancellation)
        { 
            await _serviceManager.followService.DeleteFollowingUserAsync(followUserId, false, cancellation);
            return NoContent();
        }

        [SwaggerOperation(
           Summary = "Delete a follower",
           Description = "You have to be log in.",
           Tags = ["Follows"]
           )]
        [HttpDelete("Follower/{followerUserId:guid}"), Authorize]
        public async Task<IActionResult> RemoveFollower(Guid followerUserId, CancellationToken cancellation)
        {
            await _serviceManager.followService.DeleteFollowingUserAsync(followerUserId, true, cancellation);
            return NoContent();
        }
    }
}
