namespace InterviewSim.Core.Dtos;

public readonly record struct QuestionResponseDto(
    string QuestionId,
    string Response,
    double? Score);
