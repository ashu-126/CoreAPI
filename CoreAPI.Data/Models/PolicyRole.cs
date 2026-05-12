using System;
using System.Collections.Generic;

namespace CoreAPI.Data.Models;

public partial class PolicyRole
{
    public int Id { get; set; }

    public int PolicyId { get; set; }

    public string RoleId { get; set; } = null!;

    public virtual Policy Policy { get; set; } = null!;

    public virtual AspNetRole Role { get; set; } = null!;
}
