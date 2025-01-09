using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Phone
{
    public Guid PhonesId { get; set; }

    public string? PhonesNum { get; set; }

    public bool? PhonesIsPriority { get; set; }

    public Guid? TypePhoneId { get; set; }

    public Guid? ProfileId { get; set; }

    public virtual Profile? Profile { get; set; }

    public virtual TypePhone? TypePhone { get; set; }
}
