
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IUserService
{
    public Task UpsertUser(User user);

    public Task<User> GetUser(string userId);
}

