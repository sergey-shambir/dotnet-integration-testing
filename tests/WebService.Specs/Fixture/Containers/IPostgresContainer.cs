namespace WebService.Specs.Fixture.Containers;

public interface IPostgresContainer
{
    public string GetConnectionString();

    public Task ExecuteSql(string sql);
}