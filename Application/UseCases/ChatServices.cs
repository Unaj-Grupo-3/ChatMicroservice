using Application.Interface;
using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using Domain.Entities;
using System;

namespace Application.UseCases
{
    public class ChatServices : IChatServices
    {
        private readonly IChatCommands _commands;
        private readonly IChatQueries _queries;
        private readonly IMessageQueries _queriesms;
        private readonly IUserApiServices _userApiServices;

        public ChatServices(IChatCommands commands, IChatQueries queries, IMessageQueries queriesms,IUserApiServices userApiServices)
        {
            _commands = commands;
            _queries = queries;
            _queriesms = queriesms;
            _userApiServices = userApiServices;
            
        }

        public async Task<ChatResponse> CreateChat(ChatRequest request)
        {
            Chat chat = new Chat()
            {
                UserId1 = request.UserId1,
                UserId2 = request.UserId2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            Chat create = await _commands.CreateChat(chat);

            ChatResponse response = new ChatResponse()
            {
                User1Id = create.UserId1,
                User2Id = create.UserId2,
                ChatId = create.Id,
                Messages = new List<MessageResponse>()
            };

            return response;
        }

        public async Task<ChatResponse> GetChatById(int id)
        {
            Chat chat = await _queries.GetChatById(id);

            if (chat == null)
            {
                return null;
            }

            IList<MessageResponse> messageResponses = new List<MessageResponse>();

            foreach (Message message in chat.Messages)
            {
                MessageResponse messageResponse = new MessageResponse
                {
                    Id = message.Id,
                    FromUserId = message.FromUserId,
                    Content = message.Content,
                    SendDateTime = message.SendDateTime,
                    IsRead = message.IsRead,
                };

                messageResponses.Add(messageResponse);
            }

            ChatResponse response = new ChatResponse()
            {
                ChatId = chat.Id,
                User2Id = chat.UserId2,
                User1Id = chat.UserId1,
                Messages = messageResponses
            };

            return response;
        }

        public async Task<UserChat> GetChatsByUserId(int userId)
        {
            List<int> ints = new List<int>();
            IList<Chat> chats = await _queries.GetChatsByUserId(userId);
            foreach (Chat chat in chats)
            {
               if(chat.UserId1 == userId)
                {
                    ints.Add(chat.UserId2);
                }
               else
                {
                    ints.Add(chat.UserId1);
                }
            }
            IList<UserResponse> users = await _userApiServices.GetUserById(ints);
            IList<ChatSimpleResponse> response = new List<ChatSimpleResponse>();
            if (chats.Count == 0)
            {
                return new UserChat();
            }
            foreach (Chat chat in chats)
            {
                LastestMessage lastestmessage = new LastestMessage();
                Paginacion pagina = new Paginacion();
                DateTime order = DateTime.MinValue;
                var user = users.FirstOrDefault(s => s.UserId == chat.UserId2 || s.UserId == chat.UserId1);
                if (chat.Messages.Count > 0)
                {
                    var message = chat.Messages[chat.Messages.Count - 1];
                    lastestmessage.fromUserId = message.FromUserId;
                    lastestmessage.Content = message.Content;               
                    lastestmessage.SendDateTime = message.SendDateTime;
                    lastestmessage.IsRead = message.IsRead;
                    
                    order = message.SendDateTime;
                   
                    var item = await _queriesms.GetListMessagesId(chat.Id);
                    pagina.PageSize = 10;
                    pagina.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(item.Count())/10));
                    pagina.TotalItems = item.Count();
                }

                ChatSimpleResponse chatResponse = new ChatSimpleResponse()
                {
                    ChatId = chat.Id,
                    UserFriend = user,
                    LatestMessage = lastestmessage.Content == null ? null : lastestmessage,
                    Paginacion = pagina.TotalItems == 0 ? null:pagina,
                    Order = order,
                };
    
                response.Add(chatResponse);
            }
            IList<ChatSimpleResponse> too = response.OrderByDescending(x=> x.Order).ToList();

            List<int> idUser1 = new List<int> { userId };
            UserChat userChat = new UserChat()
            {
                UserMe = (await _userApiServices.GetUserById(idUser1))[0],
                ListChat = too,
            };
               
            return userChat;
        }

    }
}
