using System;
using System.Collections.Generic;

namespace Interconn.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public DateTime CreatedTime { get; set; }
}
