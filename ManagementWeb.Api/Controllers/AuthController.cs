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
                return BadRequest("Username or User ID is already exists.");
            return Ok(user);
        }
        [HttpGet("checkUsername/{Username}")]
        public async Task<IActionResult> CheckUsername(String Username, MyContext dbContext)
        {
            var user = await dbContext.Users.Where(c => c.Username == Username).FirstOrDefaultAsync();
            if (user is not null)
            {
                if (user.ResetPassword == null || user.ResetPasswordExpiryTime < DateTime.Now)
                {
                    return Ok(false);
                }
                return Ok(true);
            }
            return Ok(false);
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
                return BadRequest("Username has already registered new password");
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
        public async Task<ActionResult<User>> resetPassword(ResetDto request, MyContext dbContext)
        {
            var user = await authService.ResetPassword(request);
            if (user is null)
            {
                return BadRequest("Username already reset Password");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        //login
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request, MyContext dbContext)
        {
            var check = dbContext.Users.Where(c => c.Username == request.Username).FirstOrDefault();
            if (check is not null)
            {
                if (check.ResetPassword is not null)
                {
                    if (check.ResetPasswordExpiryTime < DateTime.Now)
                    {
                        return BadRequest("Please contact admin");
                    }
                    else
                    {
                        if (check.ResetPassword == request.Password)
                        {
                            return Ok(true);
                        }
                        else
                        {
                            return BadRequest("Invalid username or password");
                        }
                    }
                }
                else
                {
                    var result = await authService.LoginAsync(request);
                    if (result is null)
                        return BadRequest("Invalid username or password.");
                    return Ok(result);
                }
            }
            return BadRequest("Invalid username or password.");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");
            return Ok(result);
        }
        [HttpGet("refresh/{id}")]
        //get expiry time
        public async Task<IActionResult> getRefreshTime(Guid id, MyContext dbContext)
        {
            var refreshTime = await dbContext.Users.Where(c => c.Id == id).Select(d => d.RefreshTokenExpiryTime).FirstAsync();
            return Ok(refreshTime);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        //show user
        public IActionResult getDetail(String id, MyContext dbContext)
        {
            User? user = dbContext.Users.Where(c => c.UserId == id).First();
            return Ok(user);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("detail/{id}")]
        //show user
        public async Task<IActionResult> getDetailId(string id, MyContext dbContext)
        {
            List<string> allUsers = new List<string>();
            var users = await dbContext.Users.Where(c => c.UserId == id).Select(c => new { c.CreateUserId, c.ResetPasswordId }).SingleAsync();
            var admin = await dbContext.Users.Where(c => c.Id == users.CreateUserId).Select(c => new { c.Rank, c.Name, c.Surname }).SingleAsync();
            allUsers.Add(admin.Rank + admin.Name + " " + admin.Surname);
            if (users.ResetPasswordId != Guid.Empty)
            {
                var adminChange = await dbContext.Users.Where(c => c.Id == users.ResetPasswordId).Select(c => new { c.Rank, c.Name, c.Surname }).SingleAsync();
                allUsers.Add(adminChange.Rank + adminChange.Name + " " + adminChange.Surname);
                return Ok(allUsers);
            }
            allUsers.Add(string.Empty);
            return Ok(allUsers);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("allUser")]
        //show all user
        public async Task<IActionResult> getAllUser(MyContext dbContext)
        {
            var user01 = await dbContext.Users.Where(c => c.Role == "user").Select(d => new { d.Id, d.Username, d.Role, d.PasswordHash, d.UserId, d.ResetPassword, d.ResetPasswordExpiryTime }).OrderBy(c => c.Username).ToListAsync();
            var user02 = await dbContext.Users.Where(c => c.Role == "inspector").Select(d => new { d.Id, d.Username, d.Role, d.PasswordHash, d.UserId, d.ResetPassword, d.ResetPasswordExpiryTime }).OrderBy(c => c.Username).ToListAsync();
            var user03 = await dbContext.Users.Where(c => c.Role == "admin").Select(d => new { d.Id, d.Username, d.Role, d.PasswordHash, d.UserId, d.ResetPassword, d.ResetPasswordExpiryTime }).OrderBy(c => c.Username).ToListAsync();
            var user04 = await dbContext.Users.Where(c => c.Role == "supervisor").Select(d => new { d.Id, d.Username, d.Role, d.PasswordHash, d.UserId, d.ResetPassword, d.ResetPasswordExpiryTime }).OrderBy(c => c.Username).ToListAsync();
            var user = user01.Concat(user02).Concat(user03).Concat(user04);
            return Ok(user);
        }
        [Authorize]
        [HttpGet("allInspector")]
        public async Task<IActionResult> getInspector(MyContext dbContext)
        {
            var user = await dbContext.Users.Where(d => d.Role == "Inspector").Select(d => new { UserId = d.UserId, Rank = d.Rank, Name = d.Name, Surname = d.Surname, Id = d.Id }).OrderBy(d => d.Name).ToArrayAsync();
            return Ok(user);
        }
        [Authorize]
        [HttpGet("allWorker")]
        public async Task<IActionResult> getWorker(MyContext dbContext)
        {
            var user01 = await dbContext.Users.Where(d => d.Role == "user" && d.Rank == "จ.ต.").Select(d => new { UserId = d.UserId, Rank = d.Rank, Name = d.Name, Surname = d.Surname }).OrderBy(d => d.Name).ToArrayAsync();
            var user02 = await dbContext.Users.Where(d => d.Role == "user" && d.Rank == "จ.ท.").Select(d => new { UserId = d.UserId, Rank = d.Rank, Name = d.Name, Surname = d.Surname }).OrderBy(d => d.Name).ToArrayAsync();
            var user03 = await dbContext.Users.Where(d => d.Role == "user" && d.Rank == "จ.อ.").Select(d => new { UserId = d.UserId, Rank = d.Rank, Name = d.Name, Surname = d.Surname }).OrderBy(d => d.Name).ToArrayAsync();
            var user04 = await dbContext.Users.Where(d => d.Role == "user" && d.Rank == "พ.อ.ต.").Select(d => new { UserId = d.UserId, Rank = d.Rank, Name = d.Name, Surname = d.Surname }).OrderBy(d => d.Name).ToArrayAsync();
            var user05 = await dbContext.Users.Where(d => d.Role == "user" && d.Rank == "พ.อ.ท.").Select(d => new { UserId = d.UserId, Rank = d.Rank, Name = d.Name, Surname = d.Surname }).OrderBy(d => d.Name).ToArrayAsync();
            var user06 = await dbContext.Users.Where(d => d.Role == "user" && d.Rank == "พ.อ.อ.").Select(d => new { UserId = d.UserId, Rank = d.Rank, Name = d.Name, Surname = d.Surname }).OrderBy(d => d.Name).ToArrayAsync();

            var user = user01.Concat(user02).Concat(user03).Concat(user04).Concat(user05).Concat(user06).ToArray();
            return Ok(user);
        }
        [Authorize]
        [HttpGet("fullName/{id}")]
        public async Task<IActionResult> fullName(Guid id, MyContext dbContext)
        {
            var user = await dbContext.Users.Where(c => c.Id == id).Select(c => new { c.Rank, c.Name, c.Surname }).FirstOrDefaultAsync();
            if (user is not null)
            {
                var fullName = user.Rank + user.Name + " " + user.Surname;
                return Ok(fullName);
            }
            return NotFound();
        }
    }
}
