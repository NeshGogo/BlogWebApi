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
            var test = HttpContext.Request;
            Guid.TryParse(User.FindFirst("Id").Value, out var userid);
            var dto = await _serviceManager.PostService.CreatePostAsync(userid, creationDto, cancellation);
            return CreatedAtAction(nameof(GetPostById), new { id = dto.Id }, dto);
        }
    }
}
