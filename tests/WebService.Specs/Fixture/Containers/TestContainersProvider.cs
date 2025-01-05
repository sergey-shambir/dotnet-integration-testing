namespace WebService.Specs.Fixture.Containers;

public static class TestContainersProvider
{
    public static readonly ITestContainersHost Instance =
        ExternalTestContainersHost.TryCreate()
        ?? (ITestContainersHost)new DefaultTestContainersHost();
}