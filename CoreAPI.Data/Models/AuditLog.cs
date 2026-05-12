using System;
using System.Collections.Generic;

namespace CoreAPI.Data.Models;

public partial class AuditLog
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Details { get; set; }
}
