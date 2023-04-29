

using Application.Interfaces;
using System.Security.Claims;

namespace Application.UseCases
{
    public class TokenServices : ITokenServices
    {
        public bool IsExpiredToken(ClaimsIdentity identity)
        {

            var exp = identity.Claims.FirstOrDefault(x => x.Type == "exp").Value;

            var expirationDateUnix = long.Parse(exp.ToString());
            var expirationDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expirationDateUnix);

            return expirationDateTimeOffset.UtcDateTime > DateTime.UtcNow;

        }

        public bool ValidateUserId(ClaimsIdentity identity, int userId)
        {
            try
            {
                var id = identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value;

                if (id != userId.ToString())
                {
                    return false;
                }

                return true;
            }

            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
