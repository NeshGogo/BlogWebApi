using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IServiceManager _service;

        public TokenController(IServiceManager service) => _service = service;

        [SwaggerOperation(
            Summary = "Get a new token from the refresh token",
            Description = "You don't required any permision to do it.",
            Tags = ["Token"]
            )]
        [HttpPost("Refresh")]
        public async Task<ActionResult<TokenDto>> Refresh([FromBody] TokenDto tokenDto)
        {
           return await _service.UserService.RefreshTokenAsync(tokenDto);
        }
    }
}
