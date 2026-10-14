
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Mundial.IntegrationTests.Infrastructure;
using Xunit;

namespace Mundial.IntegrationTests.Controllers;

public class PedidosCompraIntegrationTests
    : AuthenticatedIntegrationTestBase, IClassFixture<MundialApiFactory>
{
    public PedidosCompraIntegrationTests(MundialApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Post_DeveCriarPedidoComFornecedorEProduto()
    {
        // Arrange: cadastrar fornecedor
        var fornecedorResponse = await Client.PostAsJsonAsync(
            "/api/fornecedores",
            new
            {
                Cnpj = "12345678000195",
                RazaoSocial = "Fornecedor do Pedido Teste Ltda"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            fornecedorResponse.StatusCode);

        var fornecedorJson = await fornecedorResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var fornecedorId = fornecedorJson.GetProperty("id").GetInt32();

        // Arrange: cadastrar produto
        var produtoResponse = await Client.PostAsJsonAsync(
            "/api/produtos",
            new
            {
                Codigo = "PED-INT-001",
                Descricao = "Produto para Pedido de Teste",
                Preco = 12.50m
            });

        Assert.Equal(
            HttpStatusCode.Created,
            produtoResponse.StatusCode);

        var produtoJson = await produtoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var produtoId = produtoJson.GetProperty("id").GetInt32();

        // Act: criar pedido de compra
        var pedidoResponse = await Client.PostAsJsonAsync(
            "/api/pedidos-compra",
            new
            {
                Numero = "PC-INT-001",
                FornecedorId = fornecedorId,
                Itens = new[]
                {
                    new
                    {
                        ProdutoId = produtoId,
                        Quantidade = 10m,
                        PrecoUnitario = 12.50m
                    }
                }
            });

        // Assert: resposta da criação
        Assert.Equal(
            HttpStatusCode.Created,
            pedidoResponse.StatusCode);

        var pedidoJson = await pedidoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var pedidoId = pedidoJson.GetProperty("id").GetInt32();

        Assert.True(pedidoId > 0);

        Assert.Equal(
            "Pedido de compra criado com sucesso.",
            pedidoJson.GetProperty("mensagem").GetString());

        // Assert: consultar o pedido criado
        var consultaResponse = await Client.GetAsync(
            $"/api/pedidos-compra/{pedidoId}");

        Assert.Equal(
            HttpStatusCode.OK,
            consultaResponse.StatusCode);

        var consultaJson = await consultaResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "PC-INT-001",
            consultaJson.GetProperty("numero").GetString());

        Assert.Equal(
            "Fornecedor do Pedido Teste Ltda",
            consultaJson.GetProperty("fornecedor").GetString());

        Assert.Equal(
            "Aberto",
            consultaJson.GetProperty("status").GetString());

        var itens = consultaJson.GetProperty("itens");

        Assert.Equal(1, itens.GetArrayLength());

        var item = itens[0];

        Assert.Equal(
            produtoId,
            item.GetProperty("produtoId").GetInt32());

        Assert.Equal(
            10m,
            item.GetProperty("quantidadeSolicitada").GetDecimal());

        Assert.Equal(
            0m,
            item.GetProperty("quantidadeRecebida").GetDecimal());

        Assert.Equal(
            10m,
            item.GetProperty("quantidadePendente").GetDecimal());

        Assert.Equal(
            12.50m,
            item.GetProperty("precoUnitario").GetDecimal());
    }
}
