using System;

namespace InterviewSim.Core.Entities;

public class RubricCriterion
{
    public int Id { get; set; }
    public string QuestionId { get; set; } = null!;
    public string Criterion { get; set; } = null!;
    public double Weight { get; set; }
    public string Excellent { get; set; } = null!;
    public string Poor { get; set; } = null!;

    public Question Question { get; set; } = null!;
}
