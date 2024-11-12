using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WebService.Specs.Fixture;

public class CustomWebApplicationFactory<TEntryPoint>(string dbConnectionString) : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    // Меняет конфигурацию до создания объекта Program, см. https://github.com/dotnet/aspnetcore/issues/37680
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>()
            {
                { "ConnectionStrings:MainConnection", dbConnectionString }
            });
        });

        return base.CreateHost(builder);
    }
}