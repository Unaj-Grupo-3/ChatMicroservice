using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
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
        public async Task<List<UserResponse>> GetUserById(List<int> userIds)
        {
            try
            {
                string urlusers = null;  
                List<UserResponse> listuser = new List<UserResponse>();
                var httpClient = _httpClientFactory.CreateClient();
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
                var responseUser = await httpClient.GetAsync("https://localhost:7020/api/v1/User/userByIds/ids?" + urlusers);

                if(responseUser.IsSuccessStatusCode)
                { 
                    //var responseObject = JsonDocument.Parse(responseContent).RootElement;
                   var responseContent = await responseUser.Content.ReadAsStringAsync();
                   List<JsonElement> users = JsonSerializer.Deserialize<List<JsonElement>>(responseContent);
                   foreach(var item in users)
                    {
                        UserResponse user = new UserResponse();
                        user.UserId = item.GetProperty("userId").GetInt32();
                        user.UserName = item.GetProperty("name").ToString();
                        user.LastName = item.GetProperty("lastName").ToString();
                        if(item.GetProperty("images").ToString().Count() > 5)
                        {
                            JArray jArray = JArray.Parse(item.GetProperty("images").ToString());
                            user.Images =  jArray[0].SelectToken("url").ToString();
                        }
                        else
                        {
                            user.Images = null; 
                        }
                        listuser.Add(user);
                    }
                    return listuser;
                }
                else 
                {
                    return null;
                }

            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

    }
}
