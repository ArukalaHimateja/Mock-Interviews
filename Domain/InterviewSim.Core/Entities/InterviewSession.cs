using System;
using System.Collections.Generic;

namespace InterviewSim.Core.Entities;

public class InterviewSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartedUtc { get; set; }
    public DateTime? EndedUtc { get; set; }
    public string Track { get; set; } = null!;
    public string Difficulty { get; set; } = null!;

    public User User { get; set; } = null!;
    public List<QuestionResponse> QuestionResponses { get; set; } = new();
}
