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
            JCH = newTask.JCH,
            Worker = newTask.Worker,
            Inspector = newTask.Inspector,
            System = newTask.System,
            Problem = newTask.Problem,
            CreatedTimeTask = DateTime.Now,
            Code = newTask.Code,
            TaskStatus = 1,
            CreatedUserId = newTask.CreatedUserId,
        };

        dbContext.Tasks.Add(task);
        dbContext.SaveChanges();
        return (Ok(new { Id = task.Id }));
    }

    [Authorize]
    [HttpGet("{id}")]
    //get task detail
    public IActionResult Get(int id, MyContext dbContext)
    {
        TaskReport? task = dbContext.Tasks.Find(id);
        return task is null ? NotFound() : Ok(task);

    }

    [Authorize(Roles = "user")]
    [HttpGet("allTask/{id}")]
    //get task with user id
    public async Task<IActionResult> GetTaskWithId(Guid id, MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.CreatedUserId == id).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "user")]
    [HttpGet("inProgress/{id}")]
    //get task that hasn't get approved or rejected yet and task that wait for rubmit repair report
    public async Task<IActionResult> GetTaskInProgressWithID(Guid id, MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.CreatedUserId == id && (c.TaskStatus == 1 || c.TaskStatus == 3 || c.TaskStatus == 4)).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("inProgress")]
    //get task that submit
    public async Task<IActionResult> GetTaskInProgress(MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.TaskStatus == 1 || c.TaskStatus == 4).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("checked")]
    //get task that approved or wait for submit repair report
    public async Task<IActionResult> GetTaskChecked(MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.TaskStatus == 2 || c.TaskStatus == 3 || c.TaskStatus == 5 || c.TaskStatus == 6).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "supervisor")]
    [HttpGet("completed")]
    //get all completed
    public async Task<IActionResult> GetCompleted(MyContext dbContext)
    {
        var tasks = await dbContext.Tasks.Where(c => c.TaskStatus == 2 || c.TaskStatus == 5).ToListAsync();
        return Ok(tasks);
    }

    [Authorize(Roles = "admin,user")]
    [HttpPut("{id}")]
    public IActionResult Put(int id, UpdateTask updateTask, MyContext dbContext)
    {
        TaskReport? task = dbContext.Tasks.Find(id);
        if (task is not null)
        {
            //admin change status report
            if (task.TaskStatus != 3)
            {
                task.ChangeStatusUserId = updateTask.ChangeStatusUserId;
                task.TaskStatus = updateTask.TaskStatus;

            }
            //after user create repair report, update repairid in task table
            else if (task.TaskStatus == 3)
            {
                task.RepairReportId = updateTask.RepairReportId;
                task.TaskStatus = 4;

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