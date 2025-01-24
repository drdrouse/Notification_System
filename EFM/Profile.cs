using System;
using System.Collections.Generic;

namespace Notification_System.EFM;

public partial class Profile
{
    public Guid ProfileId { get; set; }

    public string ProfileSurname { get; set; } = null!;

    public string ProfileName { get; set; } = null!;

    public string ProfilePatronymic { get; set; } = null!;

    public int ProfileTabNum { get; set; }

    public byte[]? ProfilePhoto { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Mail> Mail { get; set; } = new List<Mail>();

    public virtual ICollection<OrganizationUnit> OrganizationUnits { get; set; } = new List<OrganizationUnit>();

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
