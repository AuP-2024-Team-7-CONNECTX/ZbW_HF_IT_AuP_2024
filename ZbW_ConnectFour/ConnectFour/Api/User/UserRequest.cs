namespace ConnectFour.Api.User
{
    public record UserRequest(
        Guid Id,
        string Name,
        string Email,
        string Password,
        bool Authenticated
    );
}
