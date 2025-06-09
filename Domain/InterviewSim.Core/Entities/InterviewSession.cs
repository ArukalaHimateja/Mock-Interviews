namespace InterviewSim.Core.Entities;

public record InterviewSession(
    Guid Id,
    Guid UserId,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyList<QuestionResponse> Responses);
