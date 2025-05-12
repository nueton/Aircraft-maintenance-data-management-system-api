using System;

namespace ManagementWeb.Api.Models.User;

public class UsernameDto
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserId { get; set; } = "";
    public string Rank { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public Guid CreateUserId { get; set; }
}
