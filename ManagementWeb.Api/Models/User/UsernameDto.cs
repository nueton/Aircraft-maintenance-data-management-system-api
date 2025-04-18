using System;

namespace ManagementWeb.Api.Models.User;

public class UsernameDto
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
