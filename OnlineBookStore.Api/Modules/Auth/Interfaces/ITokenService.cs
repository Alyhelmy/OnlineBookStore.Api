using OnlineBookStore.Api.Modules.Auth.Models;

namespace OnlineBookStore.Api.Modules.Auth.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
