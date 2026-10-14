
using Mundial.Application.DTOs.NotasFiscais;
using Mundial.Application.Exceptions;
using Mundial.Application.Interfaces;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;
using Mundial.Domain.Enums;

namespace Mundial.Application.Services;

public class RecebimentoFiscalService
{
    private readonly IPedidoCompraRepository _pedidoRepository;
    private readonly IAcordoComercialRepository _acordoRepository;
    private readonly IDocumentoFiscalRepository _documentoRepository;
    private readonly INotaFiscalXmlParser _xmlParser;
    private readonly IUnitOfWork _unitOfWork;

    public RecebimentoFiscalService(
        IPedidoCompraRepository pedidoRepository,
        IDocumentoFiscalRepository documentoRepository,
        INotaFiscalXmlParser xmlParser,
        IAcordoComercialRepository acordoRepository,
        IUnitOfWork unitOfWork)
    {
        _pedidoRepository = pedidoRepository;
        _documentoRepository = documentoRepository;
        _xmlParser = xmlParser;
        _acordoRepository = acordoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotaFiscalXmlDto> ValidarXmlAsync(
        Stream xmlStream,
        CancellationToken cancellationToken = default)
    {
        NotaFiscalXmlDto nota;

        try
        {
            nota = _xmlParser.Parse(xmlStream);
        }
        catch (ArgumentException ex)
        {
            throw new RegraNegocioException(ex.Message);
        }

        if (nota.ChaveAcesso.Length != 44 ||
            !nota.ChaveAcesso.All(char.IsDigit))
        {
            throw new RegraNegocioException(
                "A chave de acesso deve conter 44 dígitos.");
        }

        if (nota.CnpjEmitente.Length != 14 ||
            !nota.CnpjEmitente.All(char.IsDigit))
        {
            throw new RegraNegocioException(
                "O CNPJ do emitente deve conter 14 dígitos.");
        }

        if (await _documentoRepository.ExistePorChaveAcessoAsync(
            nota.ChaveAcesso, cancellationToken))
        {
            throw new RegraNegocioException(
                "Esta nota fiscal já foi importada.");
        }

        var pedido = await _pedidoRepository.ObterPorNumeroAsync(
            nota.NumeroPedido, cancellationToken);

        if (pedido is null)
        {
            throw new RegraNegocioException(
                "O pedido de compra informado não existe.");
        }

        if (pedido.Status == StatusPedido.Fechado)
        {
            throw new RegraNegocioException(
                "Não é possível receber mercadorias em um pedido fechado.");
        }

        if (!string.Equals(
            pedido.Fornecedor.Cnpj,
            nota.CnpjEmitente,
            StringComparison.Ordinal))
        {
            throw new RegraNegocioException(
                "O CNPJ do emitente não corresponde ao fornecedor do pedido.");
        }

        var codigosXml = nota.Itens
            .Select(x => x.CodigoProduto)
            .ToList();

        if (codigosXml.Distinct(StringComparer.Ordinal).Count()
            != codigosXml.Count)
        {
            throw new RegraNegocioException(
                "O XML possui códigos de produto repetidos.");
        }

        foreach (var item in nota.Itens)
        {
            var itemPedido = pedido.Itens.FirstOrDefault(
                x => x.Produto.Codigo == item.CodigoProduto);

            if (itemPedido is null)
            {
                throw new RegraNegocioException(
                    $"O produto {item.CodigoProduto} não pertence ao pedido.");
            }

            if (!itemPedido.Produto.PermiteRecebimento())
            {
                throw new RegraNegocioException(
                    $"O produto {item.CodigoProduto} está inativo e não pode ser recebido.");
            }

            if (item.Quantidade <= 0)
            {
                throw new RegraNegocioException(
                    $"A quantidade do produto {item.CodigoProduto} deve ser maior que zero.");
            }

            if (item.Preco < 0)
            {
                throw new RegraNegocioException(
                    $"O preço do produto {item.CodigoProduto} não pode ser negativo.");
            }

            if (item.Quantidade > itemPedido.QuantidadePendente)
            {
                throw new RegraNegocioException(
                    $"A quantidade do produto {item.CodigoProduto} excede a quantidade pendente.");
            }

            if (itemPedido.Produto.Estoque is null)
            {
                throw new RegraNegocioException(
                    $"O estoque do produto {item.CodigoProduto} não foi encontrado.");
            }
        }

        return nota;
    }

    public async Task ProcessarXmlAsync(
        Stream xmlStream,
        CancellationToken cancellationToken = default)
    {
        var nota = await ValidarXmlAsync(
            xmlStream, cancellationToken);

        var pedido = await _pedidoRepository.ObterPorNumeroAsync(
            nota.NumeroPedido, cancellationToken);

        if (pedido is null)
        {
            throw new RegraNegocioException(
                "O pedido de compra informado não existe.");
        }

        await _unitOfWork.IniciarTransacaoAsync(cancellationToken);

        try
        {
            var documento = new DocumentoFiscal(
                nota.ChaveAcesso,
                nota.CnpjEmitente,
                nota.DataEmissao,
                pedido);

            foreach (var itemNota in nota.Itens)
            {
                var itemPedido = pedido.Itens.First(
                    x => x.Produto.Codigo == itemNota.CodigoProduto);

                var produto = itemPedido.Produto;

                documento.Itens.Add(
                    new ItemDocumentoFiscal(
                        produto,
                        itemNota.Quantidade,
                        itemNota.Preco));

                if (produto.Estoque is null)
                {
                    throw new RegraNegocioException(
                        $"O estoque do produto {produto.Codigo} não foi encontrado.");
                }

                produto.Estoque.Adicionar(itemNota.Quantidade);

                itemPedido.RegistrarRecebimento(itemNota.Quantidade);

                var divergencia =
                    (itemNota.Preco - itemPedido.PrecoUnitario)
                    * itemNota.Quantidade;

                if (divergencia > 0)
                {
                    var acordo = new AcordoComercial(
                        pedido.Id,
                        produto.Id,
                        divergencia,
                        $"Preço da nota fiscal superior ao negociado para o produto {produto.Codigo}.");

                    await _acordoRepository.AdicionarAsync(
                        acordo, cancellationToken);
                }
            }

            pedido.AtualizarStatus();

            await _documentoRepository.AdicionarAsync(
                documento, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.ConfirmarTransacaoAsync(
                cancellationToken);
        }
        catch
        {
            await _unitOfWork.ReverterTransacaoAsync(
                CancellationToken.None);

            throw;
        }
    }
}
