namespace WebService.Specs.Fixture.Containers;

/// <summary>
///  Абстрагирует способ запуска вспомогательных контейнеров интеграционных тестов.
/// </summary>
public interface ITestContainersHost
{
    public Task StartAsync();

    public Task DisposeAsync();

    public string GetConnectionString();
}