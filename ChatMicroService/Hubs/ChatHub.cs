using Application.Interface;
using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using ChatMicroService.Helpers;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatMicroService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatServices _chatServices;
        private readonly ITokenServices _tokenServices;
        private readonly IMessageQuery _messageServices;
        private readonly IMessageCommands _messageCommands;


        public ChatHub(IChatServices chatServices, ITokenServices tokenServices, IMessageQuery messageServices, IMessageCommands messageCommands)
        {
            _chatServices = chatServices;
            _tokenServices = tokenServices;
            _messageServices = messageServices;
            _messageCommands = messageCommands;
        }

        [Authorize]
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

        [Authorize]
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
                await Clients.Caller.SendAsync("ErrorSendMessage", "No puede acceder a este chat");
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

        [Authorize]
        public async Task reead(int Id)
        {
           await _messageCommands.UpdateIsReadMessage(Id);             
        }
    }
}
