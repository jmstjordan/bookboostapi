using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

public class OpenAiService : IAiService
{
    private string _apiKey;
    private ILogger<OpenAiService> _logger;

    public OpenAiService(IOptions<OpenAiSettings> settings, ILogger<OpenAiService> logger)
    {
        _apiKey = settings.Value.ApiKey;
        _logger = logger;
    }

    public async Task<string> TrimDescription(string desciption, int length)
    {
        ChatClient client = new(model: "gpt-4o", apiKey: _apiKey);
        _logger.LogInformation($"Trimming description from {desciption.Length} to {length}");
        ChatCompletion completion = await client.CompleteChatAsync($"Trim the following description to {length} characters: {desciption}");
        return completion.Content[0].Text;
    }
}
    
