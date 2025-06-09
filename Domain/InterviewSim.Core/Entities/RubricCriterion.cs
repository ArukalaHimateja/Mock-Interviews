namespace InterviewSim.Core.Entities;

public record RubricCriterion(
    string Criterion,
    double Weight,
    string Excellent,
    string Poor);
