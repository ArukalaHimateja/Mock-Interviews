using System;
using System.Collections.Generic;

namespace InterviewSim.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string HashedPassword { get; set; } = null!;
    public DateTime CreatedUtc { get; set; }

    public Credit? Credit { get; set; }
    public List<InterviewSession> Sessions { get; set; } = new();
}
