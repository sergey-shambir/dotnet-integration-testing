using Reqnroll;
using Testcontainers.PostgreSql;

namespace WebService.Specs.Fixture;

[Binding]
public class TestServerFixtureCore
{
    public static readonly TestServerFixtureCore Instance = new();

    private HttpClient? _httpClient;

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16.4-alpine")
        .WithDatabase("warehouse")
        .WithTmpfsMount("/var/lib/postgresql/data")
        .Build();

    public HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("Fixture was not initialized");

    private TestServerFixtureCore()
    {
    }

    [BeforeTestRun]
    public static Task BeforeTestRun() => Instance.InitializeAsync();

    [AfterTestRun]
    public static Task AfterTestRun() => Instance.DisposeAsync();

    private async Task InitializeAsync()
    {
        await _container.StartAsync();

        CustomWebApplicationFactory<Program> factory = new(_container.GetConnectionString());
        _httpClient = factory.CreateClient();
    }

    private async Task DisposeAsync()
    {
        _httpClient = null;
        await _container.DisposeAsync();
    }
}