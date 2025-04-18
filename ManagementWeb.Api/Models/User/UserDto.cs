using System;

namespace ManagementWeb.Api.Models.User;

public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
