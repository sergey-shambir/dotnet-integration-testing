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

    public Task<TemporaryDatabase> CreateDatabase(string databaseName)
    {
        return TemporaryDatabase.Create(new PostgresContainerProxy(_container), databaseName);
    }

    private class PostgresContainerProxy(PostgreSqlContainer container) : IPostgresContainer
    {
        public string GetConnectionString()
        {
            return container.GetConnectionString();
        }

        public Task ExecuteSql(string sql)
        {
            return container.ExecScriptAsync(sql);
        }
    }
}