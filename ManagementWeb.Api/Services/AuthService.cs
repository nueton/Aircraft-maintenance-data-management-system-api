using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ManagementWeb.Api.Contexts;
using ManagementWeb.Api.Entities;
using ManagementWeb.Api.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManagementWeb.Api.Services;

public class AuthService(MyContext context, IConfiguration configuration) : IAuthService
{
    public async Task<TokenResponseDto?> LoginAsync(UserDto request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user is null)
        {
            return null;
        }
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }
        return await CreateTokenResponse(user);
    }

    private async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        return new TokenResponseDto
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        };
    }

    public async Task<User?> RegisterAsync(UserDto request)
    {
        if (await context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return null;
        }

        var user = new User();
        var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);
        user.Username = request.Username;
        user.PasswordHash = hashedPassword;

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
    public async Task<User?> RegisterUsername(UsernameDto request)
    {
        if (await context.Users.AnyAsync(u => u.Username == request.Username || u.UserId == request.UserId))
        {
            return null;
        }

        var user = new User();
        user.Username = request.Username;
        user.Role = request.Role;
        user.Rank = request.Rank;
        user.Name = request.Name;
        user.Surname = request.Surname;
        user.UserId = request.UserId;
        user.CreateUserId = request.CreateUserId;
        user.CreatedTime = DateTime.Now;
        user.ResetPassword = RandomNumber();
        user.ResetPasswordExpiryTime = DateTime.Now.AddDays(5);

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;

    }
    private string RandomNumber()
    {
        var randomNumber = new byte[8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<User?> RegisterPassword(PasswordDto request)
    {
        User? user = context.Users.Find(request.Id);
        if (user is not null)
        {
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.PasswordHash = hashedPassword;
            user.ResetPassword = null;
            user.ResetPasswordExpiryTime = null;
            await context.SaveChangesAsync();
            return user;
        }
        return null;
    }
    public async Task<User?> ResetPassword(ResetDto request)
    {
        User? user = context.Users.Find(request.Id);
        if (user is not null)
        {
            if (user.PasswordHash is "" && user.ResetPassword != null && user.ResetPasswordExpiryTime > DateTime.Now)
            {
                return null;
            }
            user.PasswordHash = "";
            user.ResetPasswordId = request.ResetPasswordId;
            user.ResetTime = DateTime.Now;
            user.ResetPassword = RandomNumber();
            user.ResetPasswordExpiryTime = DateTime.Now.AddDays(5);
            await context.SaveChangesAsync();
            return user;
        }
        return null;

    }
    public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
    {
        var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
        if (user is null)
            return null;
        return await CreateTokenResponse(user);
    }

    private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var user = await context.Users.FindAsync(userId);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }
        return user;
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();
        return refreshToken;
    }
    private string CreateToken(User user)
    {
        var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

}
