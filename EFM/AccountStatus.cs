using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class AccountStatus
{
    public Guid AccountStatusId { get; set; }

    public string AccountStatusName { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<AutoActionJob> AutoActionJobs { get; set; } = new List<AutoActionJob>();
}
