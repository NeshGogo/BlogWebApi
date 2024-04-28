using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [Route("api/Posts/{postId:guid}/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CommentsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [SwaggerOperation(
            Summary = "Get post all comments",
            Description = "You have to be log in.",
            Tags = ["Comments"]
            )]
        [HttpGet(), Authorize]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetAllComments(Guid postId, CancellationToken cancellation = default)
            => (await _serviceManager.CommentService.GetAllCommentsByPost(postId, cancellation)).ToList();

        [SwaggerOperation(
            Summary = "Add comment to a post",
            Description = "You have to be log in.",
            Tags = ["Comments"]
            )]
        [HttpPost(), Authorize]
        public async Task<ActionResult<CommentDto>> AddComment(Guid postId, [FromBody] CommentForCreationDto creationDto, CancellationToken cancellation)        
           =>  await _serviceManager.CommentService.CreateComment(postId, creationDto, cancellation);
        

    }
}
