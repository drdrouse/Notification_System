using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class OrganizationUnit
{
    public Guid OrganizationUnitId { get; set; }

    public Guid? SubdivisionId { get; set; }

    public Guid? PostId { get; set; }

    public Guid? ProfileId { get; set; }

    public virtual Post? Post { get; set; }

    public virtual Profile? Profile { get; set; }

    public virtual Subdivision? Subdivision { get; set; }
}
