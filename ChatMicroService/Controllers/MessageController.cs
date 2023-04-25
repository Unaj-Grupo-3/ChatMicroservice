using Application.Interface;
using Application.Reponsive;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class MessageController : Controller
    {
        IMessageFunction _messageFunction;

        public MessageController(IMessageFunction messageFunction)
        {
            _messageFunction = messageFunction;

        }

        [HttpPost("Initialize")]
        public async Task<ActionResult> Initialize([FromBody] MessageInitalizeRequest request)
        {
            var response = new MessageInitalizeResponse
            {
                Messages = await _messageFunction.GetMessages(request.FromUserId, request.ToUserId)
            };

            return Ok(response);
        }
    }
}
