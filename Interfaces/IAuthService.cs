
using BookBoostApi.Models;
namespace BookBoostApi.Interfaces;

public interface IAuthService
{

    public string GenerateJwtToken(User user);

}

