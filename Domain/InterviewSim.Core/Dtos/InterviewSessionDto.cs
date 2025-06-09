namespace InterviewSim.Core.Dtos;

public readonly record struct InterviewSessionDto(
    Guid Id,
    Guid UserId,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyList<QuestionResponseDto> Responses);
