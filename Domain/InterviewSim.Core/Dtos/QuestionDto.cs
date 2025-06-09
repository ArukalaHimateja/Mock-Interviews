namespace InterviewSim.Core.Dtos;

using InterviewSim.Core.Entities;

public readonly record struct QuestionDto(
    string Id,
    string Category,
    string Difficulty,
    string Prompt,
    string? ReferenceSolution,
    IReadOnlyList<RubricCriterionDto> Rubric);
