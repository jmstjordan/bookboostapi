
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IUserService
{

    public Task<User> GetUser(string userId);

    public Task CreatUser(User user);
    
    public Task<User> GetUserbyEmail(string email);

    public Task<bool> UpdatePreferences(string userId, Preferences preferences);

    public Task UpdateRole(string userId, Role role);

    public Task UpdatePassword(string userId, string passwordHash);
}

