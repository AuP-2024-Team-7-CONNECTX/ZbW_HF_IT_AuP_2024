namespace ConnectFour.Api.User
{
    public record UserResponse(
       Guid Id,
        string Name,
        string Email,
        string Password,
        bool Authenticated
    );
}
