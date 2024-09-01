namespace Domain.ConfigurationModels;

public class OpenAIConfiguration
{
    public const string SectionName = "OpenAIConfig";

    public string Endpoint { get; set; }
    public string Key { get; set; }
    public string Deployment { get; set; }
    public int MaxTokens { get; set; }
    public int Temperature { get; set; }
    public int N { get; set; }
}
