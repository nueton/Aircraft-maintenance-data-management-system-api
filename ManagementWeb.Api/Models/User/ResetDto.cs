using System;

namespace ManagementWeb.Api.Models.User;

public class ResetDto
{
    public Guid Id { get; set; }
    public Guid ResetPasswordId { get; set; }
}
