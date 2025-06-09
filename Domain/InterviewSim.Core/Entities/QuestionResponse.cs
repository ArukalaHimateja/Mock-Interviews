namespace InterviewSim.Core.Entities;

public record QuestionResponse(
    string QuestionId,
    string Response,
    double? Score);
