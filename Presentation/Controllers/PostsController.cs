﻿using Microsoft.AspNetCore.Authorization;
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
            Summary = "Create a post",
            Description = "You have to be log in.",
            Tags = ["Posts"]
            )]
        [HttpPost, Authorize]       
        public async Task<IActionResult> CreatePost([FromForm] PostForCreationDto creationDto, CancellationToken cancellation)
        {
            Guid.TryParse(User.FindFirst("Id").Value, out var userid);
            var dto = await _serviceManager.PostService.CreatePostAsync(userid, creationDto, cancellation); 
            return Ok(dto);
        }
    }
}
