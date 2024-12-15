using Reqnroll;
using WebService.Specs.Fixture;
using WebService.Specs.Fixture.Containers;

namespace WebService.Specs.Hooks;

[Binding]
public static class TestRunHooks
{
    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        await TestContainersProvider.Instance.StartAsync();
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        await TestServerFixtureCore.DisposeInstances();
        await TestContainersProvider.Instance.DisposeAsync();
    }
}