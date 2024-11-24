using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using WebService.Specs.Fixture.Containers;

namespace WebService.Specs.Fixture;

[Binding]
public class TestServerFixtureCore : IAsyncDisposable
{
    private static readonly ConcurrentDictionary<int, TestServerFixtureCore> InstanceMap = [];

    private readonly ITestContainersHost _testContainersHost;
    private HttpClient? _httpClient;
    private ScenarioTransaction? _scenarioTransaction;
    private bool _initialized;

    public static TestServerFixtureCore Instance => InstanceMap.GetOrAdd(
        Environment.CurrentManagedThreadId,
        _ => new TestServerFixtureCore()
    );

    public HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("Fixture was not initialized");

    public async Task InitializeScenario()
    {
        if (!_initialized)
        {
            await _testContainersHost.StartAsync();
            CustomWebApplicationFactory<Program> factory = new(AttachDbContext, _testContainersHost.GetConnectionString());
            _httpClient = factory.CreateClient();
            _initialized = true;
        }

        _scenarioTransaction = await ScenarioTransaction.Create(_testContainersHost.GetConnectionString());
    }

    public async Task ShutdownScenario()
    {
        if (_scenarioTransaction == null)
        {
            throw new InvalidOperationException("Test scenario is not running");
        }

        await _scenarioTransaction!.DisposeAsync();
        _scenarioTransaction = null;
    }

    private TestServerFixtureCore()
    {
        _testContainersHost = ExternalTestContainersHost.TryCreate() ??
                              (ITestContainersHost)new DefaultTestContainersHost();
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        foreach (TestServerFixtureCore instance in InstanceMap.Values)
        {
            await instance.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient = null;
        await _testContainersHost.DisposeAsync();
    }

    private void AttachDbContext(DbContext dbContext)
    {
        if (!_initialized)
        {
            return;
        }

        if (_scenarioTransaction == null)
        {
            throw new InvalidOperationException("Test scenario is not running");
        }

        _scenarioTransaction.AttachDbContext(dbContext);
    }
}