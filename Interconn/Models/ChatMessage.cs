using System;
using System.Collections.Generic;

namespace Interconn.Models;

public partial class ChatMessage
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Message { get; set; }

    public DateTime CreatedTime { get; set; }
}
