
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Mundial.IntegrationTests.Infrastructure;
using Xunit;

namespace Mundial.IntegrationTests.Controllers;

public class NotasFiscaisIntegrationTests
    : AuthenticatedIntegrationTestBase, IClassFixture<MundialApiFactory>
{
    public NotasFiscaisIntegrationTests(MundialApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Importar_DeveAtualizarEstoqueEQuantidadeRecebida()
    {
        // Arrange: cadastrar fornecedor
        const string cnpj = "12345678000195";

        var fornecedorResponse = await Client.PostAsJsonAsync(
            "/api/fornecedores",
            new
            {
                Cnpj = cnpj,
                RazaoSocial = "Fornecedor Fiscal Teste Ltda"
            });

        Assert.Equal(HttpStatusCode.Created, fornecedorResponse.StatusCode);

        var fornecedorJson = await fornecedorResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var fornecedorId = fornecedorJson.GetProperty("id").GetInt32();

        // Arrange: cadastrar produto
        const string codigoProduto = "NF-INT-001";

        var produtoResponse = await Client.PostAsJsonAsync(
            "/api/produtos",
            new
            {
                Codigo = codigoProduto,
                Descricao = "Produto para Recebimento Fiscal",
                Preco = 12.50m
            });

        Assert.Equal(HttpStatusCode.Created, produtoResponse.StatusCode);

        var produtoJson = await produtoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var produtoId = produtoJson.GetProperty("id").GetInt32();

        // Arrange: criar pedido de compra
        var pedidoResponse = await Client.PostAsJsonAsync(
            "/api/pedidos-compra",
            new
            {
                Numero = "PC-NF-INT-001",
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

        Assert.Equal(HttpStatusCode.Created, pedidoResponse.StatusCode);

        var pedidoJson = await pedidoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var pedidoId = pedidoJson.GetProperty("id").GetInt32();

        // Arrange: montar XML aceito pelo parser
        var xml = $"""
            <?xml version="1.0" encoding="utf-8"?>
            <NotaFiscal>
              <ChaveAcesso>12345678901234567890123456789012345678901234</ChaveAcesso>
              <CNPJEmitente>{cnpj}</CNPJEmitente>
              <DataEmissao>2026-10-14T10:00:00</DataEmissao>
              <NumeroPedido>PC-NF-INT-001</NumeroPedido>
              <Itens>
                <Item>
                  <CodigoProduto>{codigoProduto}</CodigoProduto>
                  <Quantidade>5</Quantidade>
                  <Preco>12.50</Preco>
                </Item>
              </Itens>
            </NotaFiscal>
            """;

        using var multipart = new MultipartFormDataContent();

        var xmlContent = new StringContent(
            xml,
            Encoding.UTF8,
            "application/xml");

        multipart.Add(xmlContent, "arquivo", "nota-teste.xml");

        // Act: importar a nota fiscal
        var importacaoResponse = await Client.PostAsync(
            "/api/notas-fiscais/importar",
            multipart);

        // Assert: importação bem-sucedida
        Assert.Equal(HttpStatusCode.OK, importacaoResponse.StatusCode);

        // Assert: consultar pedido para verificar recebimento e estoque
        var consultaResponse = await Client.GetAsync(
            $"/api/pedidos-compra/{pedidoId}");

        Assert.Equal(HttpStatusCode.OK, consultaResponse.StatusCode);

        var consultaJson = await consultaResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Parcial",
            consultaJson.GetProperty("status").GetString());

        var item = consultaJson.GetProperty("itens")[0];

        Assert.Equal(
            5m,
            item.GetProperty("quantidadeRecebida").GetDecimal());

        Assert.Equal(
            5m,
            item.GetProperty("quantidadePendente").GetDecimal());

        Assert.Equal(
            5m,
            item.GetProperty("estoqueAtual").GetDecimal());
    }

    [Fact]
    public async Task Importar_MesmaNotaDuasVezes_NaoDeveDuplicarRecebimento()
    {
        const string cnpj = "98765432000110";
        const string codigoProduto = "NF-DUP-001";
        const string numeroPedido = "PC-NF-DUP-001";
        const string chaveAcesso =
            "22345678901234567890123456789012345678901234";

        // Cadastrar fornecedor
        var fornecedorResponse = await Client.PostAsJsonAsync(
            "/api/fornecedores",
            new
            {
                Cnpj = cnpj,
                RazaoSocial = "Fornecedor Duplicidade Teste Ltda"
            });

        Assert.Equal(HttpStatusCode.Created, fornecedorResponse.StatusCode);

        var fornecedorJson = await fornecedorResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var fornecedorId = fornecedorJson.GetProperty("id").GetInt32();

        // Cadastrar produto
        var produtoResponse = await Client.PostAsJsonAsync(
            "/api/produtos",
            new
            {
                Codigo = codigoProduto,
                Descricao = "Produto para Teste de Duplicidade",
                Preco = 10m
            });

        Assert.Equal(HttpStatusCode.Created, produtoResponse.StatusCode);

        var produtoJson = await produtoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var produtoId = produtoJson.GetProperty("id").GetInt32();

        // Criar pedido com 10 unidades
        var pedidoResponse = await Client.PostAsJsonAsync(
            "/api/pedidos-compra",
            new
            {
                Numero = numeroPedido,
                FornecedorId = fornecedorId,
                Itens = new[]
                {
                    new
                    {
                        ProdutoId = produtoId,
                        Quantidade = 10m,
                        PrecoUnitario = 10m
                    }
                }
            });

        Assert.Equal(HttpStatusCode.Created, pedidoResponse.StatusCode);

        var pedidoJson = await pedidoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var pedidoId = pedidoJson.GetProperty("id").GetInt32();

        // Preparar a mesma nota para as duas importações
        var xml = $"""
            <?xml version="1.0" encoding="utf-8"?>
            <NotaFiscal>
              <ChaveAcesso>{chaveAcesso}</ChaveAcesso>
              <CNPJEmitente>{cnpj}</CNPJEmitente>
              <DataEmissao>2026-10-14T10:00:00</DataEmissao>
              <NumeroPedido>{numeroPedido}</NumeroPedido>
              <Itens>
                <Item>
                  <CodigoProduto>{codigoProduto}</CodigoProduto>
                  <Quantidade>4</Quantidade>
                  <Preco>10</Preco>
                </Item>
              </Itens>
            </NotaFiscal>
            """;

        async Task<HttpResponseMessage> ImportarNotaAsync()
        {
            var multipart = new MultipartFormDataContent();

            multipart.Add(
                new StringContent(xml, Encoding.UTF8, "application/xml"),
                "arquivo",
                "nota-duplicada.xml");

            try
            {
                return await Client.PostAsync(
                    "/api/notas-fiscais/importar",
                    multipart);
            }
            finally
            {
                multipart.Dispose();
            }
        }

        // Primeira importação
        using var primeiraImportacao = await ImportarNotaAsync();

        Assert.Equal(HttpStatusCode.OK, primeiraImportacao.StatusCode);

        // Segunda importação: a mesma chave de acesso
        using var segundaImportacao = await ImportarNotaAsync();

        Assert.Equal(HttpStatusCode.BadRequest, segundaImportacao.StatusCode);

        // Verificar que o pedido e o estoque não foram alterados novamente
        var consultaResponse = await Client.GetAsync(
            $"/api/pedidos-compra/{pedidoId}");

        Assert.Equal(HttpStatusCode.OK, consultaResponse.StatusCode);

        var consultaJson = await consultaResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var item = consultaJson.GetProperty("itens")[0];

        Assert.Equal(
            4m,
            item.GetProperty("quantidadeRecebida").GetDecimal());

        Assert.Equal(
            6m,
            item.GetProperty("quantidadePendente").GetDecimal());

        Assert.Equal(
            4m,
            item.GetProperty("estoqueAtual").GetDecimal());
    }

    [Fact]
    public async Task Importar_ComPrecoSuperior_DeveCriarAcordoComercial()
    {
        const string cnpj = "11122233000181";
        const string codigoProduto = "NF-ACORDO-001";
        const string numeroPedido = "PC-NF-ACORDO-001";
        const string chaveAcesso =
            "32345678901234567890123456789012345678901234";

        // 1. Cadastrar fornecedor
        var fornecedorResponse = await Client.PostAsJsonAsync(
            "/api/fornecedores",
            new
            {
                Cnpj = cnpj,
                RazaoSocial = "Fornecedor Acordo Teste Ltda"
            });

        Assert.Equal(HttpStatusCode.Created, fornecedorResponse.StatusCode);

        var fornecedorJson = await fornecedorResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var fornecedorId = fornecedorJson.GetProperty("id").GetInt32();

        // 2. Cadastrar produto
        var produtoResponse = await Client.PostAsJsonAsync(
            "/api/produtos",
            new
            {
                Codigo = codigoProduto,
                Descricao = "Produto para Teste de Acordo",
                Preco = 10m
            });

        Assert.Equal(HttpStatusCode.Created, produtoResponse.StatusCode);

        var produtoJson = await produtoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var produtoId = produtoJson.GetProperty("id").GetInt32();

        // 3. Criar pedido: 4 unidades a R$ 10,00
        var pedidoResponse = await Client.PostAsJsonAsync(
            "/api/pedidos-compra",
            new
            {
                Numero = numeroPedido,
                FornecedorId = fornecedorId,
                Itens = new[]
                {
                    new
                    {
                        ProdutoId = produtoId,
                        Quantidade = 4m,
                        PrecoUnitario = 10m
                    }
                }
            });

        Assert.Equal(HttpStatusCode.Created, pedidoResponse.StatusCode);

        var pedidoJson = await pedidoResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var pedidoId = pedidoJson.GetProperty("id").GetInt32();

        // 4. Preparar nota fiscal: 4 unidades a R$ 12,00
        var xml = $"""
            <?xml version="1.0" encoding="utf-8"?>
            <NotaFiscal>
              <ChaveAcesso>{chaveAcesso}</ChaveAcesso>
              <CNPJEmitente>{cnpj}</CNPJEmitente>
              <DataEmissao>2026-10-14T10:00:00</DataEmissao>
              <NumeroPedido>{numeroPedido}</NumeroPedido>
              <Itens>
                <Item>
                  <CodigoProduto>{codigoProduto}</CodigoProduto>
                  <Quantidade>4</Quantidade>
                  <Preco>12.00</Preco>
                </Item>
              </Itens>
            </NotaFiscal>
            """;

        using var multipart = new MultipartFormDataContent();

        multipart.Add(
            new StringContent(xml, Encoding.UTF8, "application/xml"),
            "arquivo",
            "nota-acordo.xml");

        // 5. Importar nota fiscal
        using var importacaoResponse = await Client.PostAsync(
            "/api/notas-fiscais/importar",
            multipart);

        Assert.Equal(HttpStatusCode.OK, importacaoResponse.StatusCode);

        // 6. Consultar acordos comerciais
        var acordosResponse = await Client.GetAsync(
            "/api/acordos-comerciais");

        Assert.Equal(HttpStatusCode.OK, acordosResponse.StatusCode);

        var acordos = await acordosResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var acordo = acordos.EnumerateArray()
            .SingleOrDefault(a =>
                a.GetProperty("pedidoCompraId").GetInt32() == pedidoId
                && a.GetProperty("produtoId").GetInt32() == produtoId);

        Assert.NotEqual(JsonValueKind.Undefined, acordo.ValueKind);

        Assert.Equal(
            8m,
            acordo.GetProperty("valorDivergencia").GetDecimal());

        Assert.Equal(
            "Pendente",
            acordo.GetProperty("status").GetString());
    }
}
