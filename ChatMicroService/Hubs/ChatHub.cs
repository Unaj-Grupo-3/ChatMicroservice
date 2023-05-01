using Application.Interface;
using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatMicroService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatServices _chatServices;
        private readonly ITokenServices _tokenServices;
        private readonly IMessageServices _messageServices;

        public ChatHub(IChatServices chatServices, ITokenServices tokenServices, IMessageServices messageServices)
        {
            _chatServices = chatServices;
            _tokenServices = tokenServices;
            _messageServices = messageServices;
        }

        public async Task SendMessage(int chatId, string message)
        {

            // Ejemplo de uso del token
            ClaimsPrincipal user = Context.User;
            Claim userIdClaim = user.FindFirst("UserId");
            int userId = int.Parse( userIdClaim.Value );

            var responseChat = await _chatServices.GetChatById(chatId);

            // Que pasa si ingreso un id de un chat que no existe?

            if (userId != responseChat.User1Id & userId != responseChat.User2Id)
            {
                await Clients.All.SendAsync("ErrorSendMessage", "No puede acceder a este chat");
            }

            if (responseChat.User1Id != userId)
            {
                responseChat.User2Id = responseChat.User1Id;
                responseChat.User1Id = userId;
            }

            MessageRequest messageRequest = new MessageRequest()
            {
                FromUserId = userId,
                ChatId = chatId,
                Content = message
            };

           MessageResponse response = await _messageServices.CreateMessage(messageRequest);

            
            await Clients.All.SendAsync("ReceiveMessage", chatId, response);
        }
    }
}
