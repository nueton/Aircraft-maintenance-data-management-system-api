using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ManagementWeb.Api.Contexts;
using ManagementWeb.Api.Entities;
using ManagementWeb.Api.Models.User;
using ManagementWeb.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManagementWeb.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        //register at the first time
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("Username is already exists.");
            return Ok(user);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("registerUsername")]
        //register username
        public async Task<ActionResult<User>> RegisterUsername(UsernameDto request)
        {
            var user = await authService.RegisterUsername(request);
            if (user is null)
                return BadRequest("Username is already exists.");
            return Ok(user);
        }

        [HttpPut("registerPassword")]
        //register password
        public async Task<ActionResult<User>> RegisterPassword(UserDto request, MyContext dbContext)
        {
            Guid userid = dbContext.Users.Where(c => c.Username == request.Username).Select(c => c.Id).FirstOrDefault();
            if (userid == Guid.Empty)
            {
                return NotFound("Username isn't exists.");
            }
            var userPassword = dbContext.Users.Where(c => c.Id == userid).Select(c => c.PasswordHash).FirstOrDefault();
            if (userPassword != "")
            {
                return BadRequest("Username didn't need to register new password");
            }
            var userdata = new PasswordDto();
            userdata.Id = userid;
            userdata.Password = request.Password;
            var user = await authService.RegisterPassword(userdata);
            return Ok(user);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("resetPassword")]
        //reset password
        public async Task<ActionResult<User>> resetPassword(string request, MyContext dbContext)
        {
            Guid userid = dbContext.Users.Where(c => c.Username == request).Select(c => c.Id).FirstOrDefault();
            if (userid == Guid.Empty)
            {
                return NotFound("Username isn't exists.");
            }
            var userPassword = dbContext.Users.Where(c => c.Id == userid).Select(c => c.PasswordHash).FirstOrDefault();
            if (userPassword == "")
            {
                return BadRequest("Username need to register new password before reset again");
            }
            var user = await authService.ResetPassword(userid);
            return Ok(user);
        }

        [HttpPost("login")]
        //login
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password.");
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");
            return Ok(result);
        }
        [Authorize]
        [HttpGet("{id}")]
        //save userid in task and repair, it need to get username with userid to show in detil report 
        public async Task<IActionResult> getUsername(Guid id, MyContext dbContext)
        {
            var username = await dbContext.Users.Where(c => c.Id == id).Select(d => d.Username).ToListAsync();
            return Ok(username);
        }
        [HttpGet("refresh/{id}")]
        //get expiry time
        public async Task<IActionResult> getRefreshTime(Guid id, MyContext dbContext)
        {
            var refreshTime = await dbContext.Users.Where(c => c.Id == id).Select(d => d.RefreshTokenExpiryTime).ToListAsync();
            return Ok(refreshTime);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("allUser")]
        //show all user
        public async Task<IActionResult> getAllUser(MyContext dbContext)
        {
            var user = await dbContext.Users.Select(d => new { Username = d.Username, Role = d.Role, PasswordHash = d.PasswordHash }).ToListAsync();
            return Ok(user);
        }

    }
}
