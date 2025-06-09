using System.Text.Json;
using InterviewSim.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewSim.Core.Data;

public class InterviewSimContext : DbContext
{
    public InterviewSimContext(DbContextOptions<InterviewSimContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Credit> Credits => Set<Credit>();
    public DbSet<InterviewSession> InterviewSessions => Set<InterviewSession>();
    public DbSet<QuestionResponse> QuestionResponses => Set<QuestionResponse>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<RubricCriterion> RubricCriteria => Set<RubricCriterion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.HashedPassword).IsRequired();
            entity.Property(e => e.CreatedUtc).IsRequired();
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithOne(u => u.Credit)
                .HasForeignKey<Credit>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InterviewSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionResponse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Session)
                .WithMany(s => s.QuestionResponses)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<RubricCriterion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Question)
                .WithMany(q => q.Rubric)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public async Task MigrateAndSeedAsync(string questionJsonPath, CancellationToken cancellationToken = default)
    {
        await Database.EnsureCreatedAsync(cancellationToken);
        if (Database.GetMigrations().Any())
        {
            await Database.MigrateAsync(cancellationToken);
        }

        if (!Questions.Any())
        {
            var text = await File.ReadAllTextAsync(questionJsonPath, cancellationToken);
            var data = JsonSerializer.Deserialize<QuestionSeed>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (data?.Questions != null)
            {
                foreach (var q in data.Questions)
                {
                    var question = new Question
                    {
                        Id = q.Id,
                        Category = q.Category,
                        Difficulty = q.Difficulty,
                        Prompt = q.Prompt,
                        ReferenceSolution = q.ReferenceSolution
                    };
                    question.Rubric.AddRange(q.Rubric.Select(r => new RubricCriterion
                    {
                        QuestionId = q.Id,
                        Criterion = r.Criterion,
                        Weight = r.Weight,
                        Excellent = r.Excellent,
                        Poor = r.Poor
                    }));
                    Questions.Add(question);
                }
                await SaveChangesAsync(cancellationToken);
            }
        }
    }

    private sealed class QuestionSeed
    {
        public List<QuestionSeedItem> Questions { get; set; } = new();
    }

    private sealed class QuestionSeedItem
    {
        public string Id { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public string? ReferenceSolution { get; set; }
        public List<RubricSeedItem> Rubric { get; set; } = new();
    }

    private sealed class RubricSeedItem
    {
        public string Criterion { get; set; } = string.Empty;
        public double Weight { get; set; }
        public string Excellent { get; set; } = string.Empty;
        public string Poor { get; set; } = string.Empty;
    }
}
