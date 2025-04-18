using System;

namespace ManagementWeb.Api.Models.User;

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }

}
