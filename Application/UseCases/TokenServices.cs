

using Application.Interfaces;
using System.Security.Claims;

namespace Application.UseCases
{
    public class TokenServices : ITokenServices
    {
        public bool ValidateUserId(ClaimsIdentity identity, Guid userId)
        {
            try
            {
                var id = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                if (id != userId)
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
        public Guid GetUserId(ClaimsIdentity identity)
        {
            var id = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);
            return id;
        }

    }
}
