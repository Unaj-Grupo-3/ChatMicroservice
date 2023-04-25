using Application.Reponsive;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistance;

namespace Infrastructure.Query
{
    public class UserFunction : IUserFunction
    {
        private readonly ChatAppContext _chatAppContext;

        public UserFunction(ChatAppContext chatAppContext)
        {
            _chatAppContext = chatAppContext;
        }

        public User GetUserById(int id)
        {
            var entity = _chatAppContext.Users
                .Where(x => x.UserId == id)
                .FirstOrDefault();

            if (entity == null) return new User();
            return entity;
            //var awayDuration = entity.IsOnline ? "" : Utilities.CalcAwayDuration(entity.LastLogonTime);
            //return new UserResponse
            //{
            //    UserName = entity.UserName,
            //    Id = entity.Id,
            //    AvatarSourceName = entity.AvatarSourceName,
            //    IsAway = awayDuration != "" ? true : false,
            //    AwayDuration = awayDuration,
            //    IsOnline = entity.IsOnline,
            //    LastLogonTime = entity.LastLogonTime
            //};
        }
    }
}

//string Password 
//string AvatarSourceName 
//bool IsOnline 
//DateTime LastLogonTime 
//string Token 
//bool IsAway 
//string AwayDuration 
//public int UserId { get; set; }int Id
//public string Name { get; set; }string UserName 

//public string LastName { get; set; }
//public DateTime Birthday { get; set; }
//public string Description { get; set; }
//public int? LocationId { get; set; }
//public string Gender { get; set; }
//public Guid AuthId { get; set; }
//public Location? Location { get; set; }
//public IList<Image>? Images { get; set; }
