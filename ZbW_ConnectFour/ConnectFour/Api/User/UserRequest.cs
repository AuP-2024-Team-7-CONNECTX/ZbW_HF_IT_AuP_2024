namespace ConnectFour.Api.User
{
    public record UserRequest(
        string Id,
        string Name,
        string Email,
        string Password,
        bool Authenticated
    );
}
