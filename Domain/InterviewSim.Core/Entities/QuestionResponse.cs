using System;

namespace InterviewSim.Core.Entities;

public class QuestionResponse
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string QuestionId { get; set; } = null!;
    public string AnswerText { get; set; } = null!;
    public string? FeedbackJson { get; set; }
    public string? ScoreJson { get; set; }
    public int DurationSec { get; set; }

    public InterviewSession Session { get; set; } = null!;
}
