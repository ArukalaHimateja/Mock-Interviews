using System;
using System.IO;
using System.Threading.Tasks;
using InterviewSim.Core.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InterviewSim.Core.Tests;

public class DataSeedingTests
{
    [Fact]
    public async Task Seed_Loads_Questions()
    {
        var options = new DbContextOptionsBuilder<InterviewSimContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        using var context = new InterviewSimContext(options);
        await context.Database.OpenConnectionAsync();
        var baseDir = AppContext.BaseDirectory;
        var path = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "Data", "questions_v1.json"));
        await context.MigrateAndSeedAsync(path);

        var count = await context.Questions.CountAsync();
        Assert.True(count > 0);
    }
}
