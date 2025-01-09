using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Account
{
    public Guid AccountId { get; set; }

    public Guid AccountStatusId { get; set; }

    public string AccountLogin { get; set; } = null!;

    public Guid ProfileId { get; set; }

    public virtual AccountStatus AccountStatus { get; set; } = null!;

    public virtual ICollection<AutoActionJob> AutoActionJobs { get; set; } = new List<AutoActionJob>();

    public virtual ICollection<Password> Passwords { get; set; } = new List<Password>();

    public virtual Profile Profile { get; set; } = null!;
}
