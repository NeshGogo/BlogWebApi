using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Presentation.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AIController(IServiceManager service) => _service = service;

        [HttpPost("text/ImageCaption")]
        public async Task<IActionResult> GenerateImageCaption([FromForm] IFormFile file) =>
            Ok(await _service.GenerativeAiService.GenerateImageCaption(file));      
    }
}
