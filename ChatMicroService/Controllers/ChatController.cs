﻿using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace  ChatMicroService.Controllers
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
        public async Task<IActionResult> CreateChat(ChatRequest request)
        {
            try
            {
                var apikey = _configuration.GetSection("ApiKey").Get<string>();
                bool valid = HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var key );
                
                if (request == null){
                    return BadRequest
                        (new { Message = "El cuerpo de la solicitud no puede ser nulo." });
                }

                if (!valid){
                   return Unauthorized();
                }

                if (valid && key.First() != apikey){
                    return new JsonResult
                        (new   { Message = "No se puede acceder. La Key es inválida" })
                        { StatusCode = 401 };
                }

                var response = await _chatServices.CreateChat(request);

                if (response == null){
                    return new JsonResult
                        (new { Message = "No se pudo crear el chat." }) 
                        { StatusCode = 404 };
                }

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
                // Obtener el UserId del token
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = HttpContext.User?.FindFirst("UserId");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "El token no contiene un UserId válido." });
                }
                var userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                // Obtener información del chat
                var response = await _chatServices.GetChatById(request.ChatId);
                if (response == null)
                {
                    return new JsonResult
                        (new { Message = $"No existe un chat con el id {request.ChatId}" }) 
                        { StatusCode = 404 };
                } 
                
                if (request.ChatId <= 0)
                {
                    return BadRequest(new { Message = "El ID del chat no es válido." });
                }

                if (request.PageSize <= 0 || request.PageIndex < 0)
                {
                    return BadRequest(new { Message = "Los valores de paginación no son válidos." });
                }

                // Validar acceso del usuario al chat
                if (response.User1Id != userId && response.User2Id != userId)
                {
                    return new JsonResult
                        (new { Message = "No esta autorizado a ver este chat" })
                        { StatusCode = 403 };
                }

                // Ajustar el orden de los usuarios en el chat
                if (response.User1Id != userId)
                {
                    response.User2Id = response.User1Id;
                    response.User1Id = userId;
                }

                // Obtener detalles de los usuarios
                List<int> userIds = new List<int> { response.User2Id, response.User1Id };              
                List<UserResponse>  users = await _userApiServices.GetUserById(userIds);

                // Construir la respuesta
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
                // Obtener UserId del token
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = HttpContext.User?.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { Message = "UserId is missing or invalid." });
                }

                int userId1 = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                var response = await _chatServices.GetChatsByUserId(userId1);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Message = ex.Message }) { StatusCode = 500 };
            }
        }
    }
}
