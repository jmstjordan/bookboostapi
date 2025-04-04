
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IUserService
{

    public Task<User> GetUser(string username);

    public Task CreatUser(User user);
    
    public Task<User> GetUserbyEmail(string email);
}

