
namespace BookBoostApi.Interfaces;

public interface IAiService
{
    public Task<string> TrimDescription(string desciption, int length);

    public Task<string> TrimTitle(string title);
}

