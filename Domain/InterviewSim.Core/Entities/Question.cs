namespace InterviewSim.Core.Entities;

public record Question(
    string Id,
    string Category,
    string Difficulty,
    string Prompt,
    string? ReferenceSolution,
    IReadOnlyList<RubricCriterion> Rubric);
