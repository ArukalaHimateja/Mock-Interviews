namespace InterviewSim.Core.Dtos;

public readonly record struct RubricCriterionDto(
    string Criterion,
    double Weight,
    string Excellent,
    string Poor);
