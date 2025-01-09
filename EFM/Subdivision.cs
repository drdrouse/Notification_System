using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Subdivision
{
    public Guid SubdivisionId { get; set; }

    public string? SubdivisionName { get; set; }

    public Guid? SubdivisionPid { get; set; }

    public string? SubdivisionPath { get; set; }

    public virtual ICollection<OrganizationUnit> OrganizationUnits { get; set; } = new List<OrganizationUnit>();
}
