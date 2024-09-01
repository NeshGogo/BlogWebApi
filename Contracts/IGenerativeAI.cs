namespace Contracts;

public interface IGenerativeAI
{
    Task<string> GenerateText(string prompt, string systemBehavior);
    Task<string> GenerateText(string prompt, string systemBehavior, byte[] content, string contentType);
}
