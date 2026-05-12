using System;
using System.Collections.Generic;

namespace CoreAPI.Data.Models;

public partial class Policy
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PolicyRole> PolicyRoles { get; set; } = new List<PolicyRole>();
}
