using Testcontainers.PostgreSql;

namespace WebService.Specs.Fixture.Containers;

/// <summary>
///  Использует TestContainers для запуска вспомогательных контейнеров.
/// </summary>
public class DefaultTestContainersHost : ITestContainersHost
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16.4-alpine")
        .WithDatabase("warehouse")
        .WithTmpfsMount("/var/lib/postgresql/data")
        .Build();

    public async Task StartAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public string GetConnectionString()
    {
        return _container.GetConnectionString();
    }
}