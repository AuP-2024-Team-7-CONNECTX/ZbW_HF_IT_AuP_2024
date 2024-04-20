namespace ConnectFour.Api.User
{
    public record UserRequest(
        string Name,
        string Email,
        bool Authenticated,
        string Password,
        // nur für sync mit Authentication nötig, das User dort und hier die gleiche ID haben muss.
        string Id = null
    );
}
