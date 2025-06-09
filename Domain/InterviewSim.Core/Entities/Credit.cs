using System;

namespace InterviewSim.Core.Entities;

public class Credit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Balance { get; set; }
    public DateTime UpdatedUtc { get; set; }

    public User User { get; set; } = null!;
}
