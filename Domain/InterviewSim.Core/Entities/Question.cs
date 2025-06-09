using System.Collections.Generic;

namespace InterviewSim.Core.Entities;

public class Question
{
    public string Id { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Difficulty { get; set; } = null!;
    public string Prompt { get; set; } = null!;
    public string? ReferenceSolution { get; set; }

    public List<RubricCriterion> Rubric { get; set; } = new();
}
