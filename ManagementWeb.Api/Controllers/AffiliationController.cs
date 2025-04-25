using Microsoft.AspNetCore.Authorization;
using ManagementWeb.Api.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManagementWeb.Api.Entities;

namespace ManagementWeb.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AffiliationController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get(MyContext dbContext)
        {
            var name = dbContext.Affiliations;
            return Ok(name);
        }
    }
}
