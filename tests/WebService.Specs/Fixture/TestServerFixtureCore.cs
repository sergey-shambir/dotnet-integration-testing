using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using WebService.Specs.Fixture.Containers;

namespace WebService.Specs.Fixture;

public class TestServerFixtureCore(int instanceId) : IAsyncDisposable
{
    private static readonly ConcurrentDictionary<int, TestServerFixtureCore> InstanceMap = [];

    private TemporaryDatabase? _database;
    private HttpClient? _httpClient;
    private ScenarioTransaction? _scenarioTransaction;
    private bool _initialized;
    private Exception? _initializationException;

    public static TestServerFixtureCore Instance => InstanceMap.GetOrAdd(
        Environment.CurrentManagedThreadId,
        instanceId => new TestServerFixtureCore(instanceId)
    );

    public HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("Fixture was not initialized");

    public async Task InitializeScenario()
    {
        await InitializeInstanceOnce();
        _scenarioTransaction = await ScenarioTransaction.Create(_database!.ConnectionString);
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

    public static async Task DisposeInstances()
    {
        foreach (TestServerFixtureCore instance in InstanceMap.Values)
        {
            await instance.DisposeAsync();
        }

        InstanceMap.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient = null;
        if (_database is not null)
        {
            await _database.DisposeAsync();
            _database = null;
        }
    }

    private async Task InitializeInstanceOnce()
    {
        if (_initializationException is not null)
        {
            throw _initializationException;
        }

        if (!_initialized)
        {
            try
            {
                await InitializeInstance();
            }
            catch (Exception e)
            {
                _initializationException = e;
                throw;
            }

            _initialized = true;
        }
    }

    private async Task InitializeInstance()
    {
        _database = await TestContainersProvider.Instance.CreateDatabase("test_" + instanceId);
        CustomWebApplicationFactory<Program> factory = new(AttachDbContext, _database.ConnectionString);
        _httpClient = factory.CreateClient();
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