using System;

namespace ManagementWeb.Api.Models.RepirReport;

public class CreateRepairReport
{
    public string DecommissionedSerial { get; set; } = "";
    public string DecommissionedParcel { get; set; } = "";
    public string CommissionedSerial { get; set; } = "";
    public string CommissionedParcel { get; set; } = "";
    public int TaskReportId { get; set; }
}
