using Npgsql;

namespace WebService.Specs.Fixture.Containers;

public sealed class TemporaryDatabase(IPostgresContainer container, string databaseName) : IAsyncDisposable
{
    public string ConnectionString { get; } = BuildConnectionString(container, databaseName);

    public static async Task<TemporaryDatabase> Create(IPostgresContainer container, string databaseName)
    {
        TemporaryDatabase database = new(container, databaseName);
        await container.ExecuteSql($"CREATE DATABASE \"{databaseName}\"");

        return database;
    }

    public async ValueTask DisposeAsync()
    {
        await container.ExecuteSql($"DROP DATABASE \"{databaseName}\"");
    }

    private static string BuildConnectionString(IPostgresContainer container, string databaseName)
    {
        NpgsqlConnectionStringBuilder builder = new(container.GetConnectionString())
        {
            Database = databaseName,
        };
        return builder.ConnectionString;
    }
}