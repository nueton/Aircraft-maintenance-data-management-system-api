using System;

namespace ManagementWeb.Api.Models.Task;

public class CreateTask
{
    public string OriginalAffiliation { get; set; } = "";
    public string DesignSpecification { get; set; } = "";
    public string JCH { get; set; } = "";
    public string Worker { get; set; } = "";
    public Guid InspectorId { get; set; }
    public string System { get; set; } = "";
    public string Problem { get; set; } = "";
    public DateTime CreatedTimeTask { get; set; }
    public string Code { get; set; } = "";
    public Guid CreatedUserId { get; set; }
}
