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
        ChatCompletion completion = await client.CompleteChatAsync($"Trim this description to {length} characters and only reply with the description: {desciption}");
        return completion.Content[0].Text;
    }

    public async Task<string> TrimTitle(string title)
    {
        ChatClient client = new(model: "gpt-4o", apiKey: _apiKey);
        ChatCompletion completion = await client.CompleteChatAsync($"Can you give me the main title of this and only reply with the main title: {title}");
        return completion.Content[0].Text;
    }
}
    
