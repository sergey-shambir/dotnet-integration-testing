namespace WebService.Specs.Fixture.Containers;

/// <summary>
///  Использует переданные через переменные окружения параметры подключения к вспомогательным контейнерам.
/// </summary>
public class ExternalTestContainersHost : ITestContainersHost
{
    private readonly string _mainConnectionString;

    private const string MainConnectionEnvVar = "TESTS_MAIN_CONNECTION";

    public static ExternalTestContainersHost? TryCreate()
    {
        string? mainConnectionString = Environment.GetEnvironmentVariable(MainConnectionEnvVar);
        if (mainConnectionString == null)
        {
            return null;
        }

        return new ExternalTestContainersHost(mainConnectionString);
    }

    private ExternalTestContainersHost(string mainConnectionString)
    {
        _mainConnectionString = mainConnectionString;
    }

    Task ITestContainersHost.StartAsync()
    {
        return Task.CompletedTask;
    }

    Task ITestContainersHost.DisposeAsync()
    {
        return Task.CompletedTask;
    }

    string ITestContainersHost.GetConnectionString()
    {
        return _mainConnectionString;
    }
}