using System;

namespace ManagementWeb.Api.Models.Task;

public class UpdateTask
{
    public int Id { get; set; }
    public int TaskStatus { get; set; }
    public Guid InspectorId { get; set; }

    public Guid AdminId { get; set; }
    public int RepairReportId { get; set; }
}
