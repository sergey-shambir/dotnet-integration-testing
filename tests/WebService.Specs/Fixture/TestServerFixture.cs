using Reqnroll;

namespace WebService.Specs.Fixture;

[Binding]
public class TestServerFixture : ITestServerFixture, IDisposable
{
    public static readonly TestServerHostPool HostPool = new(size: 2);

    private readonly TestServerHost _host = HostPool.Acquire();

    public HttpClient HttpClient => _host.HttpClient;

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        await _host.InitializeScenario();
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        await _host.ShutdownScenario();
    }

    public async ValueTask DisposeAsync()
    {
        await _host.DisposeAsync();
    }

    public void Dispose()
    {
        HostPool.Release(_host);
    }
}