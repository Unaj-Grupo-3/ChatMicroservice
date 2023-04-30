using Application.Interfaces;
using System.Text.Json;

namespace Application.UseCases
{
    public class UserApiServices : IUserApiServices
    {
        private string? _message;
        private string? _response;
        private int _statusCode;
        private string? _url;
        private HttpClient _httpClient;

        public UserApiServices()
        {
            _url = "https://localhost:7020/api/v1/User/Auth/";
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(handler);
        }

        public async Task<bool> GetUserByAuthId(Guid authId)
        {
            try
            {
                var responseUser = await _httpClient.GetAsync(_url + authId);
                var responseContent = await responseUser.Content.ReadAsStringAsync();
                var responseObject = JsonDocument.Parse(responseContent).RootElement;

                _message = responseObject.GetProperty("message").GetString();
                _response = responseObject.GetProperty("response").ToString();
                _statusCode = (int)responseUser.StatusCode;

                return responseUser.IsSuccessStatusCode;
            }
            catch (System.Net.Http.HttpRequestException)
            {
                _message = "Error en el microservicio de usuario";
                _statusCode = 500;
                return false;
            }
        }

        public string GetMessage()
        {
            return _message;
        }

        public JsonDocument GetResponse()
        {
            if (_response == null)
            {
                return JsonDocument.Parse("{}");
            }

            return JsonDocument.Parse(_response);
        }

        public int GetStatusCode()
        {
            return _statusCode;
        }

    }
}
