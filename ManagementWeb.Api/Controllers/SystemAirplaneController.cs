using ManagementWeb.Api.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManagementWeb.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SystemAirplaneController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get(MyContext dbContext)
        {
            var name = dbContext.Systems;
            return Ok(name);
        }
    }
}
