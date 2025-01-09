using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class AutoActionJob
{
    public Guid AutoActionJobId { get; set; }

    public DateTime? AutoActionJobStartDate { get; set; }

    public Guid? AccountId { get; set; }

    public Guid? AccountStatusId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual AccountStatus? AccountStatus { get; set; }
}
