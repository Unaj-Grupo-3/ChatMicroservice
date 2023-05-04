

using System.Security.Claims;

namespace Application.Interfaces
{
    public interface ITokenServices
    {
        bool ValidateUserId(ClaimsIdentity identity, Guid userId);
        Guid GetUserId(ClaimsIdentity identity);
    }
}
