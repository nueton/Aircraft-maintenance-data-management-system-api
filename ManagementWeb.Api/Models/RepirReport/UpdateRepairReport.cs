using System;

namespace ManagementWeb.Api.Models.RepirReport;

public class UpdateRepairReport
{
    public int Id { get; set; }
    public string DecommissionedSerial { get; set; } = "";
    public string DecommissionedParcel { get; set; } = "";
    public string CommissionedSerial { get; set; } = "";
    public string CommissionedParcel { get; set; } = "";
    public DateTime CreatedTimeRepair { get; set; }
    public Guid ChangeStatusUserId { get; set; }
    public int RepairStatus { get; set; }
}
