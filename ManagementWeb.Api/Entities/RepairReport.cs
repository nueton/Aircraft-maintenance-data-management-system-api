using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementWeb.Api.Entities;

public class RepairReport
{
    public int Id { get; set; }
    public string DecommissionedSerial { get; set; } = "";
    public string DecommissionedParcel { get; set; } = "";
    public string CommissionedSerial { get; set; } = "";
    public string CommissionedParcel { get; set; } = "";
    public int RepairStatus { get; set; }
    public DateTime CreatedTimeRepair { get; set; }
    public Guid ChangeStatusUserId { get; set; }

    public int TaskReportId { get; set; }
}
