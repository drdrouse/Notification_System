using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Mail
{
    public Guid MailsId { get; set; }

    public string? MailsName { get; set; }

    public Guid? ProfileId { get; set; }

    public virtual Profile? Profile { get; set; }
}
