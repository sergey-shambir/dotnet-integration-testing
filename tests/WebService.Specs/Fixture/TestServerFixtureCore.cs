using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using Testcontainers.PostgreSql;

namespace WebService.Specs.Fixture;

[Binding]
public class TestServerFixtureCore : IAsyncDisposable
{
    private static readonly ConcurrentDictionary<int,TestServerFixtureCore> InstanceMap = [];

    private HttpClient? _httpClient;
    private ScenarioTransaction? _scenarioTransaction;
    private bool _initialized;

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16.4-alpine")
        .WithDatabase("warehouse")
        .WithTmpfsMount("/var/lib/postgresql/data")
        .Build();

    public static TestServerFixtureCore Instance => InstanceMap.GetOrAdd(
        Environment.CurrentManagedThreadId,
        _ => new TestServerFixtureCore()
    );

    public HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("Fixture was not initialized");

    public async Task InitializeScenario()
    {
        if (!_initialized)
        {
            await _container.StartAsync();
            CustomWebApplicationFactory<Program> factory = new(AttachDbContext, _container.GetConnectionString());
            _httpClient = factory.CreateClient();
            _initialized = true;
        }

        _scenarioTransaction = await ScenarioTransaction.Create(_container.GetConnectionString());
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
        await _container.DisposeAsync();
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