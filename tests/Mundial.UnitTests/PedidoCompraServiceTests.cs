
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Mundial.Application.DTOs.Pedidos;
using Mundial.Application.Interfaces;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Application.Services;
using Mundial.Domain.Entities;
using Mundial.Domain.Enums;
using Xunit;

namespace Mundial.UnitTests;

public class PedidoCompraServiceTests
{
    private readonly Mock<IPedidoCompraRepository> _pedidoRepository = new();
    private readonly Mock<IFornecedorRepository> _fornecedorRepository = new();
    private readonly Mock<IProdutoRepository> _produtoRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly PedidoCompraService _service;

    public PedidoCompraServiceTests()
    {
        _service = new PedidoCompraService(
            _pedidoRepository.Object,
            _fornecedorRepository.Object,
            _produtoRepository.Object,
            _unitOfWork.Object);
    }

    private static Fornecedor CriarFornecedor()
    {
        return new Fornecedor("58464982000152", "Fornecedor Teste");
    }

    private static Produto CriarProduto(string codigo = "TESTE-001")
    {
        var produto = new Produto(codigo, "Produto de Teste", 10m);

        // Simula o ID que seria atribuído pelo banco de dados.
        typeof(Produto)
            .GetProperty(nameof(Produto.Id))!
            .SetValue(produto, 1);

        return produto;
    }

    private void ConfigurarFornecedor(Fornecedor fornecedor)
    {
        _fornecedorRepository
            .Setup(x => x.ObterPorIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fornecedor);
    }

    private void ConfigurarPedidoInexistente()
    {
        _pedidoRepository
            .Setup(x => x.ObterPorNumeroAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PedidoCompra?)null);
    }

    private void ConfigurarProdutos(params Produto[] produtos)
    {
        _produtoRepository
            .Setup(x => x.ObterPorIdsAsync(
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(produtos.ToList());
    }

    private static CriarPedidoRequest CriarRequisicao(int produtoId = 1)
    {
        return new CriarPedidoRequest(
            "PED-TESTE-001",
            1,
            new List<CriarItemPedidoRequest>
            {
                new(produtoId, 5m, 8m)
            });
    }

    [Fact]
    public async Task CriarAsync_ProdutoAtivo_DeveCriarPedido()
    {
        var fornecedor = CriarFornecedor();
        var produto = CriarProduto();

        ConfigurarFornecedor(fornecedor);
        ConfigurarPedidoInexistente();
        ConfigurarProdutos(produto);

        var resultado = await _service.CriarAsync(CriarRequisicao());

        Assert.Equal(0, resultado);

        _pedidoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<PedidoCompra>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ProdutoSuspenso_DeveBloquearPedido()
    {
        var fornecedor = CriarFornecedor();
        var produto = CriarProduto();
        produto.AtualizarStatus(StatusProduto.SuspensoParaCompra);

        ConfigurarFornecedor(fornecedor);
        ConfigurarPedidoInexistente();
        ConfigurarProdutos(produto);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarAsync(CriarRequisicao()));

        Assert.Contains("não permite novas compras", excecao.Message);

        _pedidoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<PedidoCompra>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ProdutoInativo_DeveBloquearPedido()
    {
        var fornecedor = CriarFornecedor();
        var produto = CriarProduto();
        produto.AtualizarStatus(StatusProduto.Inativo);

        ConfigurarFornecedor(fornecedor);
        ConfigurarPedidoInexistente();
        ConfigurarProdutos(produto);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarAsync(CriarRequisicao()));

        _pedidoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<PedidoCompra>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_FornecedorInexistente_DeveBloquearPedido()
    {
        _fornecedorRepository
            .Setup(x => x.ObterPorIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Fornecedor?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarAsync(CriarRequisicao()));

        _pedidoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<PedidoCompra>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    
    [Fact]
    public async Task CriarAsync_FornecedorInativo_DeveBloquearPedido()
    {
        var fornecedor = CriarFornecedor();
        fornecedor.Desativar();

        ConfigurarFornecedor(fornecedor);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarAsync(CriarRequisicao()));

        _pedidoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<PedidoCompra>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task CriarAsync_NumeroDuplicado_DeveBloquearPedido()
    {
        var fornecedor = CriarFornecedor();
        var pedidoExistente = new PedidoCompra(
            "PED-TESTE-001",
            fornecedor);

        ConfigurarFornecedor(fornecedor);

        _pedidoRepository
        .Setup(x => x.ObterPorNumeroAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(pedidoExistente);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarAsync(CriarRequisicao()));

        _pedidoRepository.Verify(
            x => x.AdicionarAsync(
                It.IsAny<PedidoCompra>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
