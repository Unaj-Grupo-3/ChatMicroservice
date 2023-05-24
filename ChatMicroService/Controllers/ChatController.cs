using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using Presentation.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class ChatController : Controller
    {
       
        private readonly IChatServices _chatServices;
        private readonly ITokenServices _tokenServices;
        private readonly IMessageServices _messageServices;
        private readonly IUserApiServices _userApiServices;
        private readonly IConfiguration _configuration;

        public ChatController(IChatServices chatServices, ITokenServices tokenServices, IMessageServices messageServices,
            IUserApiServices userApiServices, IConfiguration configuration)
        {
            _chatServices = chatServices;
            _tokenServices = tokenServices;
            _messageServices = messageServices;
            _userApiServices = userApiServices;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiKeySchemeOptions.Scheme)]
        public async Task<IActionResult> CreateChat(ChatRequest request)
        {
            try
            {
                var apikey = _configuration.GetSection("ApiKey").Get<string>();
                var key = HttpContext.User.Identity.Name;

                if (key!=null && key != apikey)
                {
                    return new JsonResult(new { Message = "No se puede acceder. La Key es inválida" }) { StatusCode = 401 };
                }

                var response = await _chatServices.CreateChat(request);

                return new JsonResult(response) { StatusCode = 201};

            }catch (Exception ex)
            {
                return new JsonResult(new { Message = ex.Message }) { StatusCode = 500 };
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetChatById([FromQuery] MessageInitalizeRequest request)
        {
            try
            {
                // Ejemplo de uso del token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                var response = await _chatServices.GetChatById(request.ChatId);
                if (response == null)
                {
                    return new JsonResult(new { Message = $"No existe un chat con el id {request.ChatId}" }) { StatusCode = 404 };
                }
                if ( !_tokenServices.ValidateUserId(identity,response.User1Id) & !_tokenServices.ValidateUserId(identity, response.User2Id))
                {
                    return new JsonResult(new { Message = "No esta autorizado a ver este chat" }) { StatusCode = 403 };
                }
                var userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                if (response.User1Id != userId)
                {
                    response.User2Id = response.User1Id;
                    response.User1Id = userId;
                }
                List<int> userIds = new List<int> { response.User2Id, response.User1Id };              
                List<UserResponse>  users = await _userApiServices.GetUserById(userIds);

                var responses = new MessageInitalizeResponse
                {
                    PageSize = request.PageSize,
                    PageIndex = request.PageIndex,
                    TotalItems = await _messageServices.GetMessagesLong(request.ChatId),
                    ChatId = response.ChatId,
                    UserMe = users.FirstOrDefault(x => x.UserId == response.User1Id),
                    UserFriend = users.FirstOrDefault(x => x.UserId == response.User2Id),
                    Messages = await _messageServices.GetMessages(request.PageSize, request.PageIndex, request.ChatId),
                    
                };
                return Ok(responses);
            }
            catch (Exception ex)
            {
                return new JsonResult(new {Message = ex.Message}) { StatusCode = 500};
            }
        }
      
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMyChats()
        { 
            try
            {
                // Ejemplo de uso del token
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                var response = await _chatServices.GetChatsByUserId(userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Message = ex.Message }) { StatusCode = 500 };
            }
        }
    }
}
