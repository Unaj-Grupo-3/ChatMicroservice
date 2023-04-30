using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Interfaces;
using Application.Models;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class ChatController : Controller
    {
       
        private readonly IChatServices _chatServices;
        private readonly ITokenServices _tokenServices;

        public ChatController(IChatServices chatServices, ITokenServices tokenServices) 
        {
            _chatServices = chatServices;
            _tokenServices = tokenServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(ChatRequest request)
        {
            try
            {
                var response = await _chatServices.CreateChat(request);

                return new JsonResult(response) { StatusCode = 201};

            }catch (Exception ex)
            {
                return new JsonResult(new { Message = ex.Message }) { StatusCode = 500 };
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetChatById(int id)
        {
            try
            {
                // Ejemplo de uso del token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                var response = await _chatServices.GetChatById(id);

                if ( !_tokenServices.ValidateUserId(identity,response.User1Id) & !_tokenServices.ValidateUserId(identity, response.User2Id))
                {
                    return new JsonResult(new { Message = "No esta autorizado a ver este chat" }) { StatusCode = 403 };
                }
                 
                // Se llama al MicroServicio User para ver al otro usuario.
                // Se mapea a cada user por sus ids.

                return Ok(response);

            }catch (Exception ex)
            {
                return new JsonResult(new {Message = ex.Message}) { StatusCode = 500};
            }
        }

        [HttpGet("me")]
        [Authorize]
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
