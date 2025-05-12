using System;
using System.ComponentModel.DataAnnotations;

namespace ManagementWeb.Api.Entities;

public class TaskReport
{
    public int Id { get; set; }
    public string OriginalAffiliation { get; set; } = "";
    public string DesignSpecification { get; set; } = "";
    public string JCN { get; set; } = "";
    public string Worker { get; set; } = "";
    public string System { get; set; } = "";
    public string Problem { get; set; } = "";
    public string Code { get; set; } = "";
    public int TaskStatus { get; set; }
    public DateTime CreatedTimeTask { get; set; }
    public Guid CreatedUserId { get; set; }
    public DateTime InspectorChangeTime { get; set; }
    public Guid InspectorId { get; set; }
    public DateTime AdminChangeTime { get; set; }
    public Guid AdminId { get; set; }
    public int RepairReportId { get; set; }
}
