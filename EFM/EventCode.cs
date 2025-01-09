using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class EventCode
{
    public Guid EventCodeId { get; set; }

    public string EventCodeDescription { get; set; } = null!;

    public string EventCodeName { get; set; } = null!;

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
}
