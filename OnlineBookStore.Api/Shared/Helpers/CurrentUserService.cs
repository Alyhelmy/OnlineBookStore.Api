using System.Security.Claims;

namespace OnlineBookStore.Api.Shared.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
    
        private readonly IHttpContextAccessor _httpContextAccessor; // we inject the IHttpContextAccessor service into the CurrentUserService class, which allows us to access the current HTTP context and retrieve information about the authenticated user.
    
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId  // This property retrieves the user ID from the JWT token claims. It uses the IHttpContextAccessor to access the current HTTP context and extract the user ID from the claims. If the user ID is not present or cannot be parsed as an integer, it returns null.
        {
            get // here we used get instead of => because we have more complex logic that involves multiple statements to retrieve and parse the user ID from the claims. The get accessor allows us to execute this logic and return the appropriate value
            {
                var userId = _httpContextAccessor.HttpContext?
                    .User?  
                    .FindFirstValue(ClaimTypes.NameIdentifier); 

                return int.TryParse(userId, out var id) ? id : null;
            }
        }

        public string? Email => // here we used => instead of get because it's a simple expression-bodied property that directly returns the value of the email claim from the JWT token. It uses the IHttpContextAccessor to access the current HTTP context and extract the email claim value. If the email claim is not present, it returns null.
            _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Email);

        public string? FullName =>
            _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Name);

        public string? Role =>
            _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Role);

    }
}
