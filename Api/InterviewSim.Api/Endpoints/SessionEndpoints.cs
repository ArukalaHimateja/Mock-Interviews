using System.Security.Claims;
using Azure.Storage.Blobs;
using InterviewSim.Core.Data;
using InterviewSim.Core.Dtos;
using InterviewSim.Core.Entities;
using InterviewSim.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace InterviewSim.Api.Endpoints;

public static class SessionEndpoints
{
    public static RouteGroupBuilder MapSessionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/sessions").RequireAuthorization();

        group.MapPost("/start", async (StartSessionRequest req, ClaimsPrincipal user, InterviewSimContext db) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var session = new InterviewSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Track = req.Track,
                Difficulty = req.Difficulty,
                StartedUtc = DateTime.UtcNow
            };
            db.InterviewSessions.Add(session);
            await db.SaveChangesAsync();

            var question = await db.Questions.Include(q => q.Rubric)
                .OrderBy(q => Guid.NewGuid())
                .Select(q => new QuestionDto(q.Id, q.Category, q.Difficulty, q.Prompt, q.ReferenceSolution,
                    q.Rubric.Select(r => new RubricCriterionDto(r.Criterion, r.Weight, r.Excellent, r.Poor)).ToList()))
                .FirstAsync();

            return Results.Ok(new { sessionId = session.Id, question });
        });

        group.MapPost("/{id:guid}/answer", async (Guid id, AnswerRequest req, InterviewSimContext db, IGradingService grader) =>
        {
            var session = await db.InterviewSessions.FirstOrDefaultAsync(s => s.Id == id);
            if (session == null) return Results.NotFound();

            var question = await db.Questions.Include(q => q.Rubric).FirstAsync(q => q.Id == req.QuestionId);
            var score = await grader.GradeAsync(question, req.Answer);

            db.QuestionResponses.Add(new QuestionResponse
            {
                Id = Guid.NewGuid(),
                SessionId = id,
                QuestionId = req.QuestionId,
                AnswerText = req.Answer,
                ScoreJson = score.ToString(),
                DurationSec = req.DurationSec
            });
            await db.SaveChangesAsync();

            var next = await db.Questions.Include(q => q.Rubric)
                .Where(q => q.Id != req.QuestionId)
                .OrderBy(q => Guid.NewGuid())
                .Select(q => new QuestionDto(q.Id, q.Category, q.Difficulty, q.Prompt, q.ReferenceSolution,
                    q.Rubric.Select(r => new RubricCriterionDto(r.Criterion, r.Weight, r.Excellent, r.Poor)).ToList()))
                .FirstOrDefaultAsync();

            return Results.Ok(new { feedback = (string?)null, nextQuestion = next });
        });

        group.MapGet("/{id:guid}/report", (Guid id, BlobServiceClient blobClient) =>
        {
            var url = $"https://example.com/reports/{id}.pdf";
            return Results.Ok(new { url });
        });

        return group;
    }

    public record StartSessionRequest(string Track, string Difficulty);
    public record AnswerRequest(string QuestionId, string Answer, int DurationSec);
}
