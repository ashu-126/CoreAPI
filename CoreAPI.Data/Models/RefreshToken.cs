using System;
using System.Collections.Generic;

namespace CoreAPI.Data.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public string? ReplacedBy { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
