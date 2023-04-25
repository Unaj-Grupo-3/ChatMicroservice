using Application.Reponsive;

namespace ChatAppWithSignalR.Api.Helpers
{
    public class UserOperator
    {
        IHttpContextAccessor _httpContext;

        public UserOperator(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public UserResponse? GetRequestUser()
        {
            if (_httpContext == null)
                return null;

            return _httpContext.HttpContext?.Items["User"] as UserResponse;
        }
    }
}
