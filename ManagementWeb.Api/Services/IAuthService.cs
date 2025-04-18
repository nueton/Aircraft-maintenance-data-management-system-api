using System;
using ManagementWeb.Api.Entities;
using ManagementWeb.Api.Models.User;

namespace ManagementWeb.Api.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto request);
    Task<User?> RegisterUsername(UsernameDto request);
    Task<User?> RegisterPassword(PasswordDto request);
    Task<User?> ResetPassword(Guid request);
    Task<TokenResponseDto?> LoginAsync(UserDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
}
