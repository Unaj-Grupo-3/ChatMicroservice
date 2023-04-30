
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IUserApiServices
    {
        Task<bool> GetUserByAuthId(Guid AuthId);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
