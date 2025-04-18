using System;

namespace ManagementWeb.Api.Models.User;

public class RefreshTokenRequestDto
{
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }

}
