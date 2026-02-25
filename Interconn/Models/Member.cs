using System;
using System.Collections.Generic;

namespace Interconn.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string Name { get; set; }

    public string Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public string Bio { get; set; }

    public string AvatarPath { get; set; }

    public DateTime UpdatedTime { get; set; }
}
