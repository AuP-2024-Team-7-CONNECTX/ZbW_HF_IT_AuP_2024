namespace ConnectFour.Api.User
{
    public record UserResponse(
       string Id,
        string Name,
        string Email,
        string Password,
        bool Authenticated,
        bool isIngame
    );
}
