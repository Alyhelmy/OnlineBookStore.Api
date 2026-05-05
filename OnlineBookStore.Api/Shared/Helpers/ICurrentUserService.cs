namespace OnlineBookStore.Api.Shared.Helpers
{
    public interface ICurrentUserService
    {
        int? UserId { get; } // This property is nullable (int?) because there may be cases where there is no authenticated user, such as when a request is made without a valid JWT token. In such cases, the UserId would be null, indicating that there is no current user context available.
        
        string? Email { get; }

        string? FullName { get; }

        string? Role { get; }

    }
}
