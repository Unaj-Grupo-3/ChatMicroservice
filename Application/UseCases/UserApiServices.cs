using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;

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
                   var responseContent = await responseUser.Content.ReadAsStringAsync();                 
                    JArray Array = JArray.Parse(responseContent);
                   foreach (var item in Array) 
                    {
                        UserResponse user = new UserResponse();
                        user.UserId = (int)item.SelectToken("userId");
                        user.UserName = (string)item.SelectToken("name");
                        user.LastName = (string)item.SelectToken("lastName");
                        JArray jArrays = (JArray)item.SelectToken("images");
                        if (!jArrays.IsNullOrEmpty())
                        {                          
                            user.Images = jArrays[0].SelectToken("url").ToString();
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
