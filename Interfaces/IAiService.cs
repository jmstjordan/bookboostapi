
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IAiService
{
    public Task<string> TrimDescription(string desciption, int length);

    public Task<string> TrimTitle(string title);

    public Task<List<string>> GetCategories(List<string> categories, List<string> genres);
}

