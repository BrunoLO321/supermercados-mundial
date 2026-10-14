
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mundial.Infrastructure.Persistence;
using Mundial.IntegrationTests.Infrastructure;
using Xunit;

namespace Mundial.IntegrationTests.Controllers;

public class FornecedoresIntegrationTests
    : AuthenticatedIntegrationTestBase, IClassFixture<MundialApiFactory>
{
    public FornecedoresIntegrationTests(MundialApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Post_DeveCadastrarFornecedorERetornar201()
    {
        // Arrange
        var request = new
        {
            Cnpj = "12345678000195",
            RazaoSocial = "Fornecedor Teste Ltda"
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/fornecedores",
            request);

        // Assert: status HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var resultado = await response.Content
            .ReadFromJsonAsync<FornecedorResponse>();

        Assert.NotNull(resultado);
        Assert.True(resultado.Id > 0);
        Assert.Equal(
            "Fornecedor cadastrado com sucesso.",
            resultado.Mensagem);

        // Assert: persistência no banco de testes
        using var scope = Factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<MundialDbContext>();

        var fornecedor = await dbContext.Fornecedores
            .AsNoTracking()
            .SingleOrDefaultAsync(f => f.Id == resultado.Id);

        Assert.NotNull(fornecedor);
        Assert.Equal(request.Cnpj, fornecedor.Cnpj);
        Assert.Equal(request.RazaoSocial, fornecedor.RazaoSocial);
    }

    private sealed record FornecedorResponse(
        int Id,
        string Mensagem);
}
