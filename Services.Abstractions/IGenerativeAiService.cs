using Microsoft.AspNetCore.Http;

namespace Services.Abstractions;

public interface IGenerativeAiService
{
    Task<string> GenerateImageCaption(IFormFile file);
}
