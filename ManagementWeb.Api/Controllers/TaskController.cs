using System.Threading.Tasks;
using ManagementWeb.Api.Contexts;
using ManagementWeb.Api.Entities;
using ManagementWeb.Api.Models;
using ManagementWeb.Api.Models.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementWeb.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    [Authorize(Roles = "user")]
    [HttpPost]
    //create task
    public IActionResult Post([FromBody] CreateTask newTask, MyContext dbContext)
    {
        TaskReport task = new()
        {
            OriginalAffiliation = newTask.OriginalAffiliation,
            DesignSpecification = newTask.DesignSpecification,
            Worker = newTask.Worker,
            InspectorId = newTask.InspectorId,
            System = newTask.System,
            Problem = newTask.Problem,
            CreatedTimeTask = DateTime.Now,
            Code = newTask.Code,
            TaskStatus = 1,
            CreatedUserId = newTask.CreatedUserId,
        };
        dbContext.Tasks.Add(task);
        dbContext.SaveChanges();
        var year = (task.CreatedTimeTask.Year + 43).ToString().Substring(2, 2);
        var month = task.CreatedTimeTask.Month.ToString();
        if (month.Length == 1)
        {
            month = "0" + month;
        }
        var day = task.CreatedTimeTask.Day.ToString();
        if (day.Length == 1)
        {
            day = "0" + day;
        }
        task.JCN = year + month + day + "-" + task.Code.Remove(2, 1) + "-" + task.Id.ToString("000");
        dbContext.SaveChanges();
        return Ok(task);
    }

    [Authorize]
    [HttpGet("{id}")]
    //get task detail
    public IActionResult Get(int id, MyContext dbContext)
    {
        TaskReport? task = dbContext.Tasks.Find(id);
        return task is null ? NotFound() : Ok(task);
    }

    [Authorize]
    [HttpGet("checkSystem/{id}")]
    public IActionResult CheckSystem(int id, MyContext dbContext)
    {
        TaskReport? task = dbContext.Tasks.Find(id);
        if (task is not null)
        {
            if (task.System == "")
            {
                return Ok(false);
            }
            else
            {
                var check = false;
                var system = task.System.Split(",");
                for (int i = 0; i < system.Length; i++)
                {
                    var repair = system[i].Split(":");
                    if (repair[1] == "true")
                    {
                        check = true;
                    }
                }
                return Ok(check);
            }
        }
        return NotFound();
    }

    [Authorize]
    [HttpGet("system/{id}")]
    //get system detail
    public IActionResult GetSystem(int id, MyContext dbContext)
    {
        TaskReport? task = dbContext.Tasks.Find(id);
        if (task is not null)
        {
            var system = task.System.Split(",");
            List<object> allSystem = new List<object>();
            for (int i = 0; i < system.Length; i++)
            {
                var repair = system[i].Split(":");
                if (repair[1] == "true")
                {
                    var addSystem = new
                    {
                        name = repair[0],
                        repair = true,
                    };
                    allSystem.Add(addSystem);
                }
            }
            return Ok(allSystem);
        }
        return NotFound();
    }

    [Authorize]
    [HttpGet("detailWithUsername/{id}")]
    //get username, inspector and admin name
    public async Task<IActionResult> GetUsername(int id, MyContext dbContext)
    {
        var detailReport = new DetailReport();
        var getMultipleId = await dbContext.Tasks.Where(c => c.Id == id).Select(c => new { CreatedUserId = c.CreatedUserId, InspectorId = c.InspectorId, AdminId = c.AdminId }).FirstOrDefaultAsync();
        if (getMultipleId != null)
        {
            var username = await dbContext.Users.Where(c => c.Id == getMultipleId.CreatedUserId).Select(d => new { Rank = d.Rank, Name = d.Name, Surname = d.Surname }).SingleOrDefaultAsync();
            var inspector = await dbContext.Users.Where(c => c.Id == getMultipleId.InspectorId).Select(d => new { Rank = d.Rank, Name = d.Name, Surname = d.Surname }).SingleOrDefaultAsync();
            if (username != null && inspector != null)
            {
                detailReport.CreatedUser = username.Rank + username.Name + " " + username.Surname;
                detailReport.Inspector = inspector.Rank + inspector.Name + " " + inspector.Surname;
            }
            if (getMultipleId.AdminId != Guid.Empty)
            {
                var admin = await dbContext.Users.Where(c => c.Id == getMultipleId.AdminId).Select(d => new { Rank = d.Rank, Name = d.Name, Surname = d.Surname }).SingleOrDefaultAsync();
                if (admin != null)
                {
                    detailReport.Admin = admin.Rank + admin.Name + " " + admin.Surname;
                }
            }
        }
        return Ok(detailReport);

    }

    [Authorize(Roles = "user")]
    [HttpGet("allTask/{id}")]
    //get task with user id
    public async Task<IActionResult> GetTaskWithId(Guid id, MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.CreatedUserId == id).ToArrayAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "user")]
    [HttpGet("inProgress/{id}")]
    //get task that hasn't get approved or rejected yet and task that wait for rubmit repair report
    public async Task<IActionResult> GetTaskInProgressWithID(Guid id, MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.CreatedUserId == id && (c.TaskStatus == 1 || c.TaskStatus == 2 || c.TaskStatus == 4 || c.TaskStatus == 5)).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "inspector")]
    [HttpGet("inspectorAllTask/{id}")]
    //get task with user id
    public async Task<IActionResult> GetTaskWithInspectorId(Guid id, MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.InspectorId == id).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "inspector")]
    [HttpGet("inspectorInProgress/{id}")]
    //get task with user id
    public async Task<IActionResult> GetTaskInProgressWithInspectorID(Guid id, MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.InspectorId == id && c.TaskStatus == 1).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("inProgress")]
    //get task that submit
    public async Task<IActionResult> GetTaskInProgress(MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.TaskStatus == 2 || c.TaskStatus == 5).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("checked")]
    //get task that approved or wait for submit repair report
    public async Task<IActionResult> GetTaskChecked(MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.TaskStatus == 1 || c.TaskStatus == 3 || c.TaskStatus == 4 || c.TaskStatus == 6 || c.TaskStatus == 7).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "supervisor")]
    [HttpGet("completed")]
    //get all completed
    public async Task<IActionResult> GetCompleted(MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.TaskStatus == 3 || c.TaskStatus == 6).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "admin,user,inspector")]
    [HttpPut("{id}")]
    public IActionResult Put(int id, UpdateTask updateTask, MyContext dbContext)
    {
        TaskReport? task = dbContext.Tasks.Find(id);
        if (task is not null)
        {
            //inspector change status report
            if (task.TaskStatus == 1)
            {
                task.InspectorId = updateTask.InspectorId;
                task.TaskStatus = updateTask.TaskStatus;
                task.InspectorChangeTime = DateTime.Now;
            }
            // admin change status
            else if (task.TaskStatus == 2)
            {
                task.AdminId = updateTask.AdminId;
                task.AdminChangeTime = DateTime.Now;
                task.TaskStatus = updateTask.TaskStatus;
            }
            dbContext.SaveChanges();
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }
}