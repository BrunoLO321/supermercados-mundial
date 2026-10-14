
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mundial.Infrastructure.Persistence;
using Mundial.IntegrationTests.Infrastructure;
using Xunit;

namespace Mundial.IntegrationTests.Controllers;

public class ProdutosIntegrationTests
    : AuthenticatedIntegrationTestBase, IClassFixture<MundialApiFactory>
{
    public ProdutosIntegrationTests(MundialApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Post_DeveCadastrarProdutoERetornar201()
    {
        // Arrange
        var request = new
        {
            Codigo = "PROD-INT-001",
            Descricao = "Arroz Tipo 1 - Teste",
            Preco = 25.90m
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/produtos",
            request);

        // Assert: resposta HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var resultado = await response.Content
            .ReadFromJsonAsync<ProdutoResponse>();

        Assert.NotNull(resultado);
        Assert.True(resultado.Id > 0);

        Assert.Equal(
            "Produto cadastrado com sucesso.",
            resultado.Mensagem);

        // Assert: persistência no SQLite em memória
        using var scope = Factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<MundialDbContext>();

        var produto = await dbContext.Produtos
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == resultado.Id);

        Assert.NotNull(produto);
        Assert.Equal(request.Codigo, produto.Codigo);
        Assert.Equal(request.Descricao, produto.Descricao);
        Assert.Equal(request.Preco, produto.Preco);
    }

    private sealed record ProdutoResponse(
        int Id,
        string Mensagem);
}
