using Application.Interface;
using Application.Models;
using Application.Reponsive;
using ChatMicroService.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatMicroService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatServices _chatServices;
        private readonly IMessageServices _messageServices;

        public ChatHub(IChatServices chatServices, IMessageServices messageServices)         
        {
            _chatServices = chatServices;          
            _messageServices = messageServices;      
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task ConnectionOn()
        {
            ClaimsPrincipal user = Context.User;
            Claim userIdClaim = user.FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);

            string connectionId = Context.ConnectionId;

            if (ConnectionHandler.ConnectedIds.ContainsKey(userId))
            {
                // Implementar cerrar la sesion.
                string connectionIdOld = ConnectionHandler.ConnectedIds[userId];

                ConnectionHandler.ConnectedIds[userId] = connectionId;

                await Clients.Clients(connectionIdOld).SendAsync("Logout");
            }
            else
            {
                ConnectionHandler.ConnectedIds.Add(userId, connectionId);
            }   
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task SendMessage(int chatId, string message)
        {
            if (message == "" || message == null)
            {
                 await Clients.Caller.SendAsync("ErrorSendMessage", "Mensaje vacio");
                 return;
            }

            // Ejemplo de uso del token
            ClaimsPrincipal user = Context.User;
            Claim userIdClaim = user.FindFirst("UserId");
            int userId = int.Parse( userIdClaim.Value );

            var responseChat = await _chatServices.GetChatById(chatId);

            // Que pasa si ingreso un id de un chat que no existe?

            if (userId != responseChat.User1Id & userId != responseChat.User2Id)
            {
                await Clients.Caller.SendAsync("ErrorSendMessage", "No puede acceder a este chat");
                return;
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

            if (ConnectionHandler.ConnectedIds.ContainsKey(responseChat.User2Id))
            {
                IReadOnlyList<string> usersToSend = new List<string>() { Context.ConnectionId, ConnectionHandler.ConnectedIds[responseChat.User2Id] };

                await Clients.Clients(usersToSend).SendAsync("ReceiveMessage",chatId, response);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", chatId, response);
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task ReadMessages(int chatId,IList<int> messageIds)
        {
            // Ejemplo de uso del token
            ClaimsPrincipal user = Context.User;
            Claim userIdClaim = user.FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);

            var responseChat = await _chatServices.GetChatById(chatId);

            // Que pasa si ingreso un id de un chat que no existe?

            if (userId != responseChat.User1Id & userId != responseChat.User2Id)
            {
                await Clients.Caller.SendAsync("ErrorSendMessage", "No puede acceder a este chat");
                return;
            }

            if (responseChat.User1Id != userId)
            {
                responseChat.User2Id = responseChat.User1Id;
                responseChat.User1Id = userId;
            }

            IList<MessageSimple> messages = await _messageServices.GetMessageByListId(messageIds);
            messages = messages.Where(x => x.ChatId == chatId && x.FromUserId != userId && !x.IsRead).ToList();

            messageIds.Clear(); 

            for (int i = 0; i < messages.Count; i++)
            {
                await _messageServices.UpdateIsRead(messages[i].Id);
                messageIds.Add(messages[i].Id);
            }

            if (ConnectionHandler.ConnectedIds.ContainsKey(responseChat.User2Id))
            {
                await Clients.Client(ConnectionHandler.ConnectedIds[responseChat.User2Id]).SendAsync("MessagesAreRead", chatId, messageIds);
            }
        }
    }
}
