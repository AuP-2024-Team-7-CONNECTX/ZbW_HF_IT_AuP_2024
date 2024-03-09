using Swashbuckle.AspNetCore.Filters;
using ConnectFour.Api.User;

public class UserRequestExample : IExamplesProvider<UserRequest>
{
    public UserRequest GetExamples()
    {
        return new UserRequest(
            Name: "Jane Doe",
            Email: "janedoe@example.com",
            Authenticated: false
        );
    }
}
