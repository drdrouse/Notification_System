using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Log
{
    public Guid LogId { get; set; }

    public Guid? EventCodeId { get; set; }

    public DateTime? LogDateTime { get; set; }

    public Guid? ProfileId { get; set; }

    public string? LogDeccription { get; set; }

    public virtual EventCode? EventCode { get; set; }

    public virtual Profile? Profile { get; set; }
}
