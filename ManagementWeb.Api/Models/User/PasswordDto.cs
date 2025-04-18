using System;

namespace ManagementWeb.Api.Models.User;

public class PasswordDto
{
    public Guid Id { get; set; }
    public string Password { get; set; } = string.Empty;
}
