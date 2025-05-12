using ManagementWeb.Api.Contexts;
using ManagementWeb.Api.Entities;
using ManagementWeb.Api.Models.RepirReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementWeb.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RepairReportController : ControllerBase
{
    [Authorize(Roles = "user")]
    [HttpPost]
    //create repair report
    public IActionResult Post([FromBody] CreateRepairReport newRepairReport, MyContext dbContext)
    {
        RepairReport repairReport = new()
        {
            DecommissionedParcel = newRepairReport.DecommissionedParcel,
            DecommissionedSerial = newRepairReport.DecommissionedSerial,
            DecommissionedNationalSerialNumber = newRepairReport.DecommissionedNationalSerialNumber,
            CommissionedParcel = newRepairReport.CommissionedParcel,
            CommissionedSerial = newRepairReport.CommissionedSerial,
            CommissionedNationalSerialNumber = newRepairReport.CommissionedNationalSerialNumber,
            RepairStatus = 5,
            CreatedTimeRepair = DateTime.Now,
            TaskReportId = newRepairReport.TaskReportId,
        };
        dbContext.RepairReports.Add(repairReport);
        dbContext.SaveChanges();
        TaskReport? task = dbContext.Tasks.Find(repairReport.TaskReportId);
        if (task is not null)
        {
            task.RepairReportId = repairReport.Id;
            task.TaskStatus = 5;
        }
        dbContext.SaveChanges();
        return Ok(repairReport);
    }

    [Authorize]
    [HttpGet("{id}")]
    //get repair report detail
    public IActionResult Get(int id, MyContext dbContext)
    {
        RepairReport? repairReport = dbContext.RepairReports.Find(id);
        return repairReport is null ? NotFound() : Ok(repairReport);
    }

    [Authorize]
    [HttpGet("system/{id}")]
    //get repair report detail
    public IActionResult GetSystem(int id, MyContext dbContext)
    {
        RepairReport? repairReport = dbContext.RepairReports.Find(id);
        if (repairReport is not null)
        {
            var decommissionedSerial = repairReport.DecommissionedSerial.Split(",");
            var decommissionedParcel = repairReport.DecommissionedParcel.Split(",");
            var decommissionedNationalSerialNumber = repairReport.DecommissionedNationalSerialNumber.Split(",");
            var commissionedSerial = repairReport.CommissionedSerial.Split(",");
            var commissionedParcel = repairReport.CommissionedParcel.Split(",");
            var commissionedNationalSerialNumber = repairReport.CommissionedNationalSerialNumber.Split(",");
            List<object> allSystem = new List<object>();
            for (int i = 0; i < decommissionedSerial.Length; i++)
            {
                var addSystem = new
                {
                    Name = decommissionedSerial[i].Split(":")[0],
                    DecommissionedSerial = decommissionedSerial[i].Split(":")[1],
                    DecommissionedParcel = decommissionedParcel[i].Split(":")[1],
                    DecommissionedNationalSerialNumber = decommissionedNationalSerialNumber[i].Split(":")[1],
                    CommissionedSerial = commissionedSerial[i].Split(":")[1],
                    CommissionedParcel = commissionedParcel[i].Split(":")[1],
                    CommissionedNationalSerialNumber = commissionedNationalSerialNumber[i].Split(":")[1]
                };
                allSystem.Add(addSystem);
            }
            return Ok(allSystem);
        }
        return NotFound();
    }

    [Authorize]
    [HttpGet("name/{id}")]
    //get name
    public IActionResult GetName(int id, MyContext dbContext)
    {
        var name = dbContext.RepairReports.Where(c => c.Id == id).Select(c => c.ChangeStatusUserId).FirstOrDefault();
        if (name != Guid.Empty)
        {
            var fullname = dbContext.Users.Where(c => c.Id == name).Select(c => new { Rank = c.Rank, Name = c.Name, Surname = c.Surname }).FirstOrDefault();
            if (fullname is not null)
            {
                return Ok(fullname.Rank + fullname.Name + " " + fullname.Surname);
            }
        }
        return Ok();
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    //change repair report status
    public IActionResult Put(int id, [FromBody] UpdateRepairReport updateRepairReport, MyContext dbContext)
    {
        RepairReport? repairReport = dbContext.RepairReports.Find(id);
        TaskReport? taskReport = dbContext.Tasks.Find(repairReport.TaskReportId);
        if (repairReport is not null && taskReport is not null)
        {
            repairReport.ChangeStatusTime = DateTime.Now;
            repairReport.RepairStatus = updateRepairReport.RepairStatus;
            repairReport.ChangeStatusUserId = updateRepairReport.ChangeStatusUserId;
            taskReport.TaskStatus = updateRepairReport.RepairStatus;
        }
        dbContext.SaveChanges();
        return Ok();
    }
}
