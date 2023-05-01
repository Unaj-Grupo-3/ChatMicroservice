using Application.Interface;
using Application.Models;
using Application.Reponsive;
using Domain.Entities;

namespace Application.UseCases
{
    public class ChatServices : IChatServices
    {
        private readonly IChatCommands _commands;
        private readonly IChatQueries _queries;

        public ChatServices(IChatCommands commands, IChatQueries queries)
        {
            _commands = commands;
            _queries = queries;
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
                    ToUserId = message.FromUserId == chat.UserId1 ? chat.UserId2 : chat.UserId1,
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

        public async Task<IList<ChatSimpleResponse>> GetChatsByUserId(int userId)
        {
            IList<Chat> chats = await _queries.GetChatsByUserId(userId);

            IList<ChatSimpleResponse> response = new List<ChatSimpleResponse>();

            if (chats.Count == 0)
            {
                return response;
            }

            foreach (Chat chat in chats)
            {
                MessageResponse messageResponse = new MessageResponse();

                if (chat.Messages.Count > 0)
                {
                    var message = chat.Messages[chat.Messages.Count - 1];
                    messageResponse.Id = message.Id;
                    messageResponse.Content = message.Content;
                    messageResponse.IsRead = message.IsRead;
                    messageResponse.SendDateTime = message.SendDateTime;
                    messageResponse.FromUserId = message.FromUserId;
                    messageResponse.ToUserId = message.FromUserId == chat.UserId1 ? chat.UserId2 : chat.UserId1;
                }

                ChatSimpleResponse chatResponse = new ChatSimpleResponse()
                {
                    ChatId = chat.Id,
                    User2Id = chat.UserId1 == userId ? chat.UserId2 : chat.UserId1,
                    LatestMessage = messageResponse.Id == 0 ? null : messageResponse,
                };
 
                response.Add(chatResponse);
            }

            return response;
        }
    }
}
