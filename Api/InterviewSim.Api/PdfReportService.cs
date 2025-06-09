using System.Globalization;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using InterviewSim.Core.Data;
using InterviewSim.Core.Entities;
using InterviewSim.Core.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InterviewSim.Api;

public class PdfReportService : IPdfReportService
{
    private readonly BlobServiceClient _blobClient;
    private readonly InterviewSimContext _db;

    public PdfReportService(BlobServiceClient blobClient, InterviewSimContext db)
    {
        _blobClient = blobClient;
        _db = db;
    }

    public async Task<string> GenerateReportAsync(InterviewSession session, CancellationToken cancellationToken = default)
    {
        var userName = session.User?.Email ?? session.UserId.ToString();

        var questions = await _db.Questions
            .Include(q => q.Rubric)
            .Where(q => session.QuestionResponses.Select(r => r.QuestionId).Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, cancellationToken);

        double overall = session.QuestionResponses.Count == 0
            ? 0
            : session.QuestionResponses.Average(r => double.TryParse(r.ScoreJson, NumberStyles.Any, CultureInfo.InvariantCulture, out var s) ? s : 0);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Content().Column(col =>
                {
                    col.Item().Text("Interview Report").FontSize(20).Bold().AlignCenter();
                    col.Item().Text($"Candidate: {userName}").FontSize(14);
                    col.Item().Text($"Overall Score: {overall:P0}").FontSize(14).Bold();
                    col.Item().Height(200).Background(Colors.Grey.Lighten2).AlignCenter().AlignMiddle().Text("Radar Chart");
                });
            });

            foreach (var resp in session.QuestionResponses)
            {
                if (!questions.TryGetValue(resp.QuestionId, out var q))
                    continue;

                var prompt = q.Prompt.Length > 200 ? q.Prompt.Substring(0, 200) + "..." : q.Prompt;
                var answer = resp.AnswerText.Length > 200 ? resp.AnswerText.Substring(0, 200) + "..." : resp.AnswerText;

                page(resp, q, prompt, answer);
            }

            void page(QuestionResponse r, Question q, string promptText, string answerText)
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Content().Column(col =>
                    {
                        col.Item().Text(promptText).FontSize(12).Bold();
                        col.Item().Text($"Answer: {answerText}").FontSize(12);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c => { c.ConstantColumn(150); c.RelativeColumn(); });
                            table.Header(h =>
                            {
                                h.Cell().Text("Criterion").FontWeight(FontWeight.Bold);
                                h.Cell().Text("Expectation").FontWeight(FontWeight.Bold);
                            });

                            foreach (var rc in q.Rubric)
                            {
                                table.Cell().Text(rc.Criterion);
                                table.Cell().Text(rc.Excellent);
                            }
                        });

                        if (!string.IsNullOrWhiteSpace(r.FeedbackJson))
                        {
                            col.Item().Text("Feedback:").FontWeight(FontWeight.Bold);
                            col.Item().Text(r.FeedbackJson);
                        }
                    });
                });
            }
        });

        await using var ms = new MemoryStream();
        document.GeneratePdf(ms);
        ms.Position = 0;

        var containerClient = _blobClient.GetBlobContainerClient("reports");
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = containerClient.GetBlobClient($"{session.Id}.pdf");
        await blobClient.UploadAsync(ms, overwrite: true, cancellationToken);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(24)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}

