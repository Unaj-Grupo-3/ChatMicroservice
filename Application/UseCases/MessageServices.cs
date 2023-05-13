using Application.Interface;
using Application.Models;
using Application.Reponsive;
using Domain.Entities;


namespace Application.UseCases
{
    public class MessageServices : IMessageServices
    {
        private readonly IMessageCommands _commands;
        private readonly IMessageQueries _queries;

        public MessageServices(IMessageCommands messageCommands, IMessageQueries messageQueries) 
        {
            _commands = messageCommands;
            _queries = messageQueries;
        }

        public async Task<MessageResponse> CreateMessage(MessageRequest request)
        {
            Message message = new Message()
            {
                FromUserId = request.FromUserId,
                ChatId = request.ChatId,
                Content = request.Content,
                IsRead = false,
                SendDateTime = DateTime.UtcNow,
            };

            Message create = await _commands.CreateMessage(message);

            MessageResponse response = new MessageResponse()
            {
                Id = create.Id,
                Content = create.Content,
                IsRead = create.IsRead,
                SendDateTime = create.SendDateTime,
                FromUserId = create.FromUserId,
            };

            return response;
        }

        public async Task<MessageResponse> UpdateIsReadMessage(int messageId)
        {
           Message messageUpdated =  await _commands.UpdateIsReadMessage(messageId);


            MessageResponse response = new MessageResponse()
            {
                Id = messageId,
                Content = messageUpdated.Content,
                IsRead = messageUpdated.IsRead,
                SendDateTime = DateTime.UtcNow,
                FromUserId = messageUpdated.FromUserId,
            };

            return response;
        }

        public async Task<IEnumerable<MessageResponse>> GetMessages(int pageSize, int pageIndex, int chatId)
        {
           var messages =await _queries.GetListMessages(pageSize,pageIndex, chatId);
            if (messages == null) messages = new List<MessageResponse>();
            return messages;
        }

        public async Task<int> GetMessagesLong(int chatId)
        { int count = 0;
            var messages = await _queries.GetListMessagesId(chatId);
            if (messages != null)
            {
                count = messages.Count();
                return count;
            }
            else
            {
                return  count;
            } 
        }
    }
}
