using InterviewSim.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace InterviewSim.Core.Tests;

public class RubricCriterionTests
{
    [Fact]
    public void Weights_Should_Sum_To_One()
    {
        var question = new Question
        {
            Id = "Q1",
            Category = "algorithms",
            Difficulty = "easy",
            Prompt = "prompt",
            Rubric = new List<RubricCriterion>
            {
                new() { QuestionId = "Q1", Criterion = "correctness", Weight = 0.5, Excellent = "good", Poor = "bad" },
                new() { QuestionId = "Q1", Criterion = "design", Weight = 0.3, Excellent = "good", Poor = "bad" },
                new() { QuestionId = "Q1", Criterion = "communication", Weight = 0.2, Excellent = "good", Poor = "bad" }
            }
        };

        var total = question.Rubric.Sum(r => r.Weight);

        Assert.Equal(1d, total, 5);
    }
}
