using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Post
{
    public Guid PostId { get; set; }

    public string? PostName { get; set; }

    public virtual ICollection<OrganizationUnit> OrganizationUnits { get; set; } = new List<OrganizationUnit>();
}
