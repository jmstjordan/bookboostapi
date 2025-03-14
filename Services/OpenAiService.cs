using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

public class OpenAiService : IAiService
{
    private string _openAiUrl;

    private string _apiKey;

    public OpenAiService(IOptions<OpenAiSettings> settings)
    {
        _openAiUrl = settings.Value.ApiUrl;
        _apiKey = settings.Value.ApiKey;
    }

    public async Task<string> TrimDescription(string desciption, int length)
    {
        ChatClient client = new(model: "gpt-4o", apiKey: _apiKey);

        ChatCompletion completion = await client.CompleteChatAsync($"Trim the following description to {length} characters: {desciption}");
        return completion.Content[0].Text;
    }
}
    
