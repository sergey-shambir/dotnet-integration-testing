using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebService.Specs.Fixture;

public class ScenarioTransaction : IAsyncDisposable
{
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;

    public static async Task<ScenarioTransaction> Create(string dbConnectionString)
    {
        NpgsqlConnection connection = new(dbConnectionString);
        await connection.OpenAsync();
        try
        {
            NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
            return new ScenarioTransaction(connection, transaction);
        }
        catch (Exception)
        {
            await connection.CloseAsync();
            throw;
        }
    }

    public void AttachDbContext(DbContext dbContext)
    {
        dbContext.Database.SetDbConnection(_connection);
        dbContext.Database.UseTransaction(_transaction);
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.RollbackAsync();
        await _connection.CloseAsync();
    }

    private ScenarioTransaction(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }
}