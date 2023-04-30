using Application.Interface;
using Application.Reponsive;

namespace Application.UseCases
{
    public class MessageServices : IMessageServices
    {
        private readonly IMessageCommands _commands;
        private readonly IMessageQueries _queries;
        public MessageServices(IMessageQueries queries,IMessageCommands commands) 
        {
            _commands = commands;
            _queries = queries;
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
            if (messages == null)
            {
                count = messages.Max(x => x.Id); 
                return count;
            }
            else
            {
                return  count;
            } 
        }
    }
}
