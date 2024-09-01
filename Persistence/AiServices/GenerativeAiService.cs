using Azure;
using Azure.AI.OpenAI;
using Contracts;
using Domain.ConfigurationModels;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Persistence.AiServices;

public sealed class GenerativeAiService : IGenerativeAI
{
    private readonly OpenAIConfiguration _config;

    public GenerativeAiService(IOptionsSnapshot<OpenAIConfiguration> configuration) => _config = configuration.Value;


    public async Task<string> GenerateText(string prompt, string systemBehavior)
    {
        var azureClient = new AzureOpenAIClient(new Uri(_config.Endpoint), new AzureKeyCredential(_config.Key));
        var chatClient = azureClient.GetChatClient(_config.Deployment);

        var chatmessage = GetChatCompletionOptions();

        var response = await chatClient.CompleteChatAsync([
            new SystemChatMessage(systemBehavior),
            new UserChatMessage(prompt)
        ]);

        return response.Value.Content[0].Text;
    }


    public async Task<string> GenerateText(string prompt, string systemBehavior, byte[] content, string contentType)
    {
        var azureClient = new AzureOpenAIClient(new Uri(_config.Endpoint), new AzureKeyCredential(_config.Key));
        var chatClient = azureClient.GetChatClient(_config.Deployment);

        var chatmessage = GetChatCompletionOptions();

        var image = ChatMessageContentPart.CreateImageMessageContentPart(new BinaryData(content), 
                contentType, ImageChatMessageContentPartDetail.Low);

        var response =  await chatClient.CompleteChatAsync([
            new SystemChatMessage(systemBehavior),
            new UserChatMessage([
                ChatMessageContentPart.CreateTextMessageContentPart(prompt),
               image,
            ])
        ]);

        return response.Value.Content[0].Text;
    }

    private ChatCompletionOptions GetChatCompletionOptions() =>
        new ChatCompletionOptions
        {
            Temperature = (float)_config.Temperature,
            MaxTokens = _config.MaxTokens,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };
}
