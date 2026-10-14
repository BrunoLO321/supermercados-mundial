
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Mundial.IntegrationTests.Infrastructure;

public abstract class AuthenticatedIntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly MundialApiFactory Factory;

    protected AuthenticatedIntegrationTestBase(MundialApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Username = "integration-admin",
                Password = "integration-password"
            });

        response.EnsureSuccessStatusCode();

        var resultado = await response.Content
            .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(resultado);
        Assert.False(string.IsNullOrWhiteSpace(resultado.AccessToken));

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", resultado.AccessToken);
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    private sealed record LoginResponse(string AccessToken);
}
