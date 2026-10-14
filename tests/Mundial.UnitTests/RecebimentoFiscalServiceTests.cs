
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Mundial.Application.DTOs.NotasFiscais;
using Mundial.Application.Exceptions;
using Mundial.Application.Interfaces;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Application.Services;
using Mundial.Domain.Entities;
using Mundial.Domain.Enums;
using Xunit;

namespace Mundial.UnitTests;

public class RecebimentoFiscalServiceTests
{
    private readonly Mock<IPedidoCompraRepository> _pedidoRepository = new();
    private readonly Mock<IDocumentoFiscalRepository> _documentoRepository = new();
    private readonly Mock<INotaFiscalXmlParser> _xmlParser = new();
    private readonly Mock<IAcordoComercialRepository> _acordoRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly RecebimentoFiscalService _service;

    private readonly Fornecedor _fornecedor;
    private readonly Produto _produto;
    private readonly PedidoCompra _pedido;

    private const string Cnpj = "58464982000152";
    private const string ChaveAcesso =
        "12345678901234567890123456789012345678901234";

    public RecebimentoFiscalServiceTests()
    {
        _fornecedor = new Fornecedor(Cnpj, "Fornecedor Teste");
        _produto = CriarProdutoComId(1);

        _pedido = new PedidoCompra("PED-TESTE-001", _fornecedor);
        _pedido.AdicionarItem(_produto, 10m, 8m);

        ConfigurarPedidoExistente();

        _documentoRepository
            .Setup(x => x.ExistePorChaveAcessoAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWork
            .Setup(x => x.IniciarTransacaoAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<IAsyncDisposable>());

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _service = new RecebimentoFiscalService(
            _pedidoRepository.Object,
            _documentoRepository.Object,
            _xmlParser.Object,
            _acordoRepository.Object,
            _unitOfWork.Object);
    }

    private static Produto CriarProdutoComId(int id)
    {
        var produto = new Produto(
            "TESTE-001",
            "Produto de Teste",
            10m);

        typeof(Produto)
            .GetProperty(nameof(Produto.Id))!
            .SetValue(produto, id);

        return produto;
    }

    private void ConfigurarPedidoExistente()
    {
        _pedidoRepository
            .Setup(x => x.ObterPorNumeroAsync(
                "PED-TESTE-001",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_pedido);
    }

    private NotaFiscalXmlDto CriarNota(
        decimal quantidade = 4m,
        decimal preco = 8m)
    {
        return new NotaFiscalXmlDto
        {
            ChaveAcesso = ChaveAcesso,
            CnpjEmitente = Cnpj,
            DataEmissao = new DateTime(2026, 10, 1),
            NumeroPedido = "PED-TESTE-001",
            Itens = new List<ItemNotaFiscalXmlDto>
            {
                new()
                {
                    CodigoProduto = "TESTE-001",
                    Quantidade = quantidade,
                    Preco = preco
                }
            }
        };
    }

    private void ConfigurarXml(NotaFiscalXmlDto nota)
    {
        _xmlParser
            .Setup(x => x.Parse(It.IsAny<Stream>()))
            .Returns(nota);
    }

    [Fact]
    public async Task ProcessarXmlAsync_NotaValida_DeveAtualizarEstoqueEPedido()
    {
        ConfigurarXml(CriarNota(quantidade: 4m, preco: 8m));

        using var stream = new MemoryStream();

        await _service.ProcessarXmlAsync(stream);

        Assert.Equal(4m, _produto.Estoque!.Quantidade);
        Assert.Equal(4m, _pedido.Itens.First().QuantidadeRecebida);

        _documentoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<DocumentoFiscal>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.ConfirmarTransacaoAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessarXmlAsync_PrecoMaiorQuePedido_DeveCriarAcordo()
    {
        ConfigurarXml(CriarNota(quantidade: 4m, preco: 10m));

        using var stream = new MemoryStream();

        await _service.ProcessarXmlAsync(stream);

        _acordoRepository.Verify(
            x => x.AdicionarAsync(
                It.Is<AcordoComercial>(a => a.ValorDivergencia == 8m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessarXmlAsync_NotaDuplicada_DeveRejeitarImportacao()
    {
        _documentoRepository
            .Setup(x => x.ExistePorChaveAcessoAsync(
                ChaveAcesso,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        ConfigurarXml(CriarNota());

        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => _service.ProcessarXmlAsync(stream));

        Assert.Equal(0m, _produto.Estoque!.Quantidade);
        Assert.Equal(0m, _pedido.Itens.First().QuantidadeRecebida);

        _documentoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<DocumentoFiscal>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessarXmlAsync_ProdutoInativo_DeveRejeitarRecebimento()
    {
        _produto.AtualizarStatus(StatusProduto.Inativo);
        ConfigurarXml(CriarNota());

        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => _service.ProcessarXmlAsync(stream));

        Assert.Equal(0m, _produto.Estoque!.Quantidade);
        Assert.Equal(0m, _pedido.Itens.First().QuantidadeRecebida);

        _documentoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<DocumentoFiscal>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessarXmlAsync_QuantidadeAcimaDoPendente_DeveRejeitarRecebimento()
    {
        ConfigurarXml(CriarNota(quantidade: 11m, preco: 8m));

        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => _service.ProcessarXmlAsync(stream));

        Assert.Equal(0m, _produto.Estoque!.Quantidade);
        Assert.Equal(0m, _pedido.Itens.First().QuantidadeRecebida);

        _documentoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<DocumentoFiscal>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.IniciarTransacaoAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessarXmlAsync_FalhaAoSalvar_DeveReverterTransacao()
    {
        ConfigurarXml(CriarNota(quantidade: 4m, preco: 8m));

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha simulada ao salvar."));

        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ProcessarXmlAsync(stream));

        _unitOfWork.Verify(
            x => x.ReverterTransacaoAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessarXmlAsync_ProdutoSuspenso_DevePermitirRecebimento()
    {
        _produto.AtualizarStatus(StatusProduto.SuspensoParaCompra);
        ConfigurarXml(CriarNota(quantidade: 4m, preco: 8m));

        using var stream = new MemoryStream();

        await _service.ProcessarXmlAsync(stream);

        Assert.Equal(4m, _produto.Estoque!.Quantidade);
        Assert.Equal(4m, _pedido.Itens.First().QuantidadeRecebida);

        _documentoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<DocumentoFiscal>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

}
