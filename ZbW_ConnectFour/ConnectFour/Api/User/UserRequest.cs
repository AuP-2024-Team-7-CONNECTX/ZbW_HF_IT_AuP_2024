﻿namespace ConnectFour.Api.User
{
    public record UserRequest(
        string Name,
        string Email,
        bool Authenticated
    );
}
