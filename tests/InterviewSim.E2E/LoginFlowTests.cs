using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace InterviewSim.E2E;

public class LoginFlowTests : PageTest
{
    [Test]
    public async Task LoginAndLoadQuestions()
    {
        await Page.GotoAsync("http://localhost:5000/login");
        await Page.FillAsync("input[label=Email]", "test@example.com");
        await Page.FillAsync("input[label=Password]", "test");
        await Page.ClickAsync("button:has-text('Login')");
        await Page.WaitForURLAsync("**/interview");
        await Expect(Page.Locator(".question-card")).ToHaveCountAsync(3);
    }
}
