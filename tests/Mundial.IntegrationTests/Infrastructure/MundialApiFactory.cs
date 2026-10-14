
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mundial.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Mundial.IntegrationTests.Infrastructure;

public class MundialApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    public MundialApiFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Username"] = "integration-admin",
                ["Auth:Password"] = "integration-password",
                ["Jwt:Key"] = "IntegrationTestKey1234567890123456789012345678901234567890",
                ["Jwt:Issuer"] = "Mundial.IntegrationTests",
                ["Jwt:Audience"] = "Mundial.IntegrationTests"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<MundialDbContext>>();

            services.AddDbContext<MundialDbContext>(options =>
                options.UseSqlite(_connection));

            services.PostConfigure<JwtBearerOptions>(options =>
{
    options.Events ??= new JwtBearerEvents();

    var eventosAnteriores = options.Events;

    eventosAnteriores.OnAuthenticationFailed = context =>
    {
        Console.WriteLine(
            $"ERRO JWT: {context.Exception}");

        return Task.CompletedTask;
    };
});

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<MundialDbContext>();

            dbContext.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
