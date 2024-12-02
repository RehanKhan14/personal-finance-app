namespace PersonalFinanceApp.Service
{
    public interface IUserSessionService
    {
        bool IsUserLoggedIn();
        int GetLoggedInUserId();
    }

    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsUserLoggedIn()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            return !string.IsNullOrEmpty(userId);
        }

        public int GetLoggedInUserId()
        {
            if (IsUserLoggedIn())
            {
                return int.Parse(_httpContextAccessor.HttpContext.Session.GetString("UserId"));
            }
            return -1;
        }
    }

}
