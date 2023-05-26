using Application.Interfaces;
using Application.Reponsive;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Application.UseCases
{
    public class UserApiServices : IUserApiServices
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private string _message;
        private bool _statusCodeIsSuccess;

        public UserApiServices(HttpClient httpClient, IConfiguration configuration)
        {
           _httpClient = httpClient;
           _apiKey = configuration["ApiKey"];
        }

        public string GetMessage()
        {
            return _message;
        }

        public bool IsSuccessStatusCode()
        {
            return _statusCodeIsSuccess;
        }

        public async Task<List<UserResponse>> GetUserById(List<int> userIds)
        {
            try
            {
                string urlusers = null;  
                List<UserResponse> listuser = new List<UserResponse>();

                foreach (var item in userIds)
                {
                    if (item == userIds[0])
                    {
                        urlusers = "usersId=" + item;
                    }
                    else
                    {
                        urlusers +=  $"&usersId={item}";
                    }
                }
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                var responseUser = await _httpClient.GetAsync("https://localhost:7020/api/v1/User/false?" + urlusers);
                var responseContent = await responseUser.Content.ReadAsStringAsync();                 

                if(responseUser.IsSuccessStatusCode)
                { 
                   JArray Array = JArray.Parse(responseContent);
                   foreach (var item in Array) 
                    {
                        UserResponse user = new UserResponse();
                        user.UserId = (int)item.SelectToken("userId");
                        user.UserName = (string)item.SelectToken("name");
                        user.LastName = (string)item.SelectToken("lastName");
                        user.Images = (string)item.SelectToken("image");
                        listuser.Add(user);
                    }
                    return listuser;
                }
                else 
                {
                    JsonDocument json = JsonDocument.Parse(responseContent);
                    _message = json.RootElement.GetProperty("message").ToString();
                    _statusCodeIsSuccess = false;
                    return null;
                }

            }
            catch (HttpRequestException)
            {
                _statusCodeIsSuccess = false;
                return null;
            }
        }

    }
}
