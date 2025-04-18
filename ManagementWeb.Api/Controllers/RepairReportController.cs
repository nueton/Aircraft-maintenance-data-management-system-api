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
            CommissionedParcel = newRepairReport.CommissionedParcel,
            CommissionedSerial = newRepairReport.CommissionedSerial,
            RepairStatus = 4,
            CreatedTimeRepair = DateTime.Now,
            TaskReportId = newRepairReport.TaskReportId,
        };

        dbContext.RepairReports.Add(repairReport);
        dbContext.SaveChanges();
        return (Ok(new { Id = repairReport.Id }));
    }

    [Authorize]
    [HttpGet("{id}")]
    //get repair report detail
    public IActionResult Get(int id, MyContext dbContext)
    {
        RepairReport? repairReport = dbContext.RepairReports.Find(id);
        return repairReport is null ? NotFound() : Ok(repairReport);
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    //change repair report status
    public IActionResult Put(int id, [FromBody] UpdateRepairReport updateRepairReport, MyContext dbContext)
    {
        RepairReport? repairReport = dbContext.RepairReports.Find(id);
        if (repairReport is not null)
        {
            repairReport.RepairStatus = updateRepairReport.RepairStatus;
            repairReport.ChangeStatusUserId = updateRepairReport.ChangeStatusUserId;
        }
        dbContext.SaveChanges();
        return Ok();
    }
}
