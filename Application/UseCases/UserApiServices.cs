using Application.Interfaces;
using Application.Reponsive;
using Domain.Entities;
using System.Net.Http;
using System.Text.Json;

namespace Application.UseCases
{
    public class UserApiServices : IUserApiServices
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserApiServices(IHttpClientFactory httpClientFactory)
        {
           _httpClientFactory = httpClientFactory;
        }

        public async Task<UserResponse> GetUserById(int id)
        {
            try
            {
                UserResponse user = new UserResponse();
                var httpClient = _httpClientFactory.CreateClient();
                var responseUser = await httpClient.GetAsync("https://localhost:7020/api/v1/User/"+ id);
               
                if(responseUser.IsSuccessStatusCode)
                { 
                   var responseContent = await responseUser.Content.ReadAsStringAsync();
                   var responseObject = JsonDocument.Parse(responseContent).RootElement;
                   user.UserId = responseObject.GetProperty("userId").GetInt32();
                   user.UserName = responseObject.GetProperty("name").ToString();
                   user.LastName = responseObject.GetProperty("lastName").ToString();
                   user.Images = responseObject.GetProperty("images").ToString();
                   return user;
                }
                else 
                { 
                   return user;
                }

            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

    }
}
