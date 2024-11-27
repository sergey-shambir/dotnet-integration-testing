using Reqnroll;

namespace WebService.Specs.Fixture;

[Binding]
public class TestServerFixture : ITestServerFixture
{
    private readonly TestServerFixtureCore _fixtureCore = TestServerFixtureCore.Instance;

    public HttpClient HttpClient => _fixtureCore.HttpClient;

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        await _fixtureCore.InitializeScenario();
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        await _fixtureCore.ShutdownScenario();
    }
}