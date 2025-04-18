using System;

namespace ManagementWeb.Api.Models.Task;

public class UpdateTask
{
    public int Id { get; set; }
    public int TaskStatus { get; set; }
    public Guid ChangeStatusUserId { get; set; }
    public int RepairReportId { get; set; }
}
