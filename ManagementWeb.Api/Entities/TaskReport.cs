using System;
using System.ComponentModel.DataAnnotations;

namespace ManagementWeb.Api.Entities;

public class TaskReport
{
    public int Id { get; set; }
    public string OriginalAffiliation { get; set; } = "";
    public string DesignSpecification { get; set; } = "";
    public string JCH { get; set; } = "";
    public string Worker { get; set; } = "";
    public string Inspector { get; set; } = "";
    public string System { get; set; } = "";
    public string Problem { get; set; } = "";
    public DateTime CreatedTimeTask { get; set; }
    public string Code { get; set; } = "";
    public int TaskStatus { get; set; }
    public Guid CreatedUserId { get; set; }
    public DateTime ChangeStatusTime { get; set; }
    public Guid ChangeStatusUserId { get; set; }
    public int RepairReportId { get; set; }
}
