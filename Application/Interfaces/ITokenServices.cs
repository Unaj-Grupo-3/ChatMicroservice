

using System.Security.Claims;

namespace Application.Interfaces
{
    public interface ITokenServices
    {
        bool IsExpiredToken(ClaimsIdentity identity);
        bool ValidateUserId(ClaimsIdentity identity, int userId);
    }
}
