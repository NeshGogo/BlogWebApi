using Contracts;
using Domain.Exceptions.File;
using Microsoft.AspNetCore.Http;
using Services.Abstractions;

namespace Services;

internal sealed class GenerativeAiService: IGenerativeAiService
{
    private readonly IGenerativeAI _generativeAI;

    public GenerativeAiService(IGenerativeAI generativeAI) => _generativeAI = generativeAI;

    public async Task<string> GenerateImageCaption(IFormFile file)
    {
        var isImage = file.ContentType.Contains("image");
        
        if (!isImage)
            throw new ImageOnlyBadRequestException();

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var content = stream.ToArray();

        var systemBehavior = "You are a helpful assistant specializing in generating " +
            "creative captions for pictures. Your captions are always short";
        var prompt = "Generate an image caption for this picture";
        
        var text = await _generativeAI.GenerateText(prompt, systemBehavior, content, file.ContentType);

        return text;
    }
}
