using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class TypePhone
{
    public Guid TypePhoneId { get; set; }

    public string? TypePhoneName { get; set; }

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
