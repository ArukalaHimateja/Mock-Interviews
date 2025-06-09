using InterviewSim.Core.Entities;
using System.Linq;
using Xunit;

namespace InterviewSim.Core.Tests;

public class RubricCriterionTests
{
    [Fact]
    public void Weights_Should_Sum_To_One()
    {
        var question = new Question(
            Id: "Q1",
            Category: "algorithms",
            Difficulty: "easy",
            Prompt: "prompt",
            ReferenceSolution: null,
            Rubric: new[]
            {
                new RubricCriterion("correctness", 0.5, "good", "bad"),
                new RubricCriterion("design", 0.3, "good", "bad"),
                new RubricCriterion("communication", 0.2, "good", "bad")
            });

        var total = question.Rubric.Sum(r => r.Weight);

        Assert.Equal(1d, total, 5);
    }
}
