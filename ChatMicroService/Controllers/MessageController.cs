using Application.Interface;
using Application.Reponsive;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class MessageController : Controller
    {
        private readonly IMessageServices _messageFunction;
        private readonly ChatAppContext _chatAppContext;
        public MessageController(IMessageServices messageFunction,ChatAppContext chatAppContext)
        {
            _messageFunction = messageFunction;
            _chatAppContext = chatAppContext;
        }

        [HttpGet({)]  
        public async Task<ActionResult> Initialize([FromQuery] MessageInitalizeRequest request)
        {
            try 
            {
                //var Items = await _messageFunction.GetMessagesLong(request.chatId);
                //if (Items == 0)
                //{
                //    return BadRequest("ChatId value invalid");
                //}
                var response = new MessageInitalizeResponse
                {
                  PageSize = request.pageSize,
                  PageIndex = request.pageIndex,
                  Messages = await _messageFunction.GetMessages(request.pageSize, request.pageIndex, request.chatId),
                  TotalItems = await _messageFunction.GetMessagesLong(request.chatId)
                };
                return Ok(response);
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }
    }
}
