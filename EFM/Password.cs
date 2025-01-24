using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Password
{
    public Guid PasswordId { get; set; }

    public string PasswordValue { get; set; } = null!;

    public DateOnly PasswordStartDate { get; set; }

    public DateOnly? PasswordStopDate { get; set; }

    public Guid AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
