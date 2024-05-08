using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PostsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [SwaggerOperation(
            Summary = "Get all post by the user or not",
            Description = "You have to be log in.",
            Tags = ["Posts"]
            )]
        [HttpGet(), Authorize]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts([FromQuery] bool me = false, CancellationToken cancellation = default)
        {
            Guid.TryParse(User.FindFirst("Id").Value, out var userId);
            var result = me 
                ? await _serviceManager.PostService.GetPostsByUserId(userId, cancellation) 
                : await _serviceManager.PostService.GetPostsAllPost(cancellation);
            return result.ToList();
        }


        [SwaggerOperation(
            Summary = "Get a post created by user",
            Description = "You have to be log in.",
            Tags = ["Posts"]
            )]
        [HttpGet("{id:guid}", Name = "GetPostById"), Authorize]
        public async Task<ActionResult<PostDto>> GetPostById(Guid id, CancellationToken cancellation)
            => await _serviceManager.PostService.GetPostByAsync(id, cancellation);

        [SwaggerOperation(
            Summary = "Create a post",
            Description = "You have to be log in.",
            Tags = ["Posts"]
            )]
        [HttpPost, Authorize]       
        public async Task<IActionResult> CreatePost([FromForm] PostForCreationDto creationDto, CancellationToken cancellation)
        {
            Guid.TryParse(User.FindFirst("Id").Value, out var userid);
            var dto = await _serviceManager.PostService.CreatePostAsync(userid, creationDto, cancellation);
            return CreatedAtAction(nameof(GetPostById), new { id = dto.Id }, dto);
        }

        [SwaggerOperation(
           Summary = "Update a post",
           Description = "You have to be log in.",
           Tags = ["Posts"]
           )]
        [HttpPut("{id:guid}"), Authorize]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] PostForUpdateDto updateDto, CancellationToken cancellation)
        {
            await _serviceManager.PostService.UpdatePostAsync(id, updateDto, cancellation);
            return NoContent();
        }

        [SwaggerOperation(
           Summary = "Add or remove like to a post",
           Description = "You have to be log in.",
           Tags = ["Posts"]
           )]
        [HttpPatch("{id:guid}/Likes"), Authorize]
        public async Task<IActionResult> AddOrRemoveLike(Guid id, CancellationToken cancellation)
        {
            await _serviceManager.PostService.AddOrRemovePostLikeAsync(id, cancellation);
            return NoContent();
        }

        [SwaggerOperation(
           Summary = "Delete a post",
           Description = "You have to be log in.",
           Tags = ["Posts"]
           )]
        [HttpDelete("{id:guid}"), Authorize]
        public async Task<IActionResult> DeletePost(Guid id, CancellationToken cancellation)
        {
            await _serviceManager.PostService.DeletePostAsync(id, cancellation);
            return NoContent();
        }
    }
}
