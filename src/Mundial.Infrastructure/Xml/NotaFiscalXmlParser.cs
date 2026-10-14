
using System.Globalization;
using System.Xml.Linq;
using Mundial.Application.DTOs.NotasFiscais;
using Mundial.Application.Interfaces;

namespace Mundial.Infrastructure.Xml;

public class NotaFiscalXmlParser : INotaFiscalXmlParser
{
    public NotaFiscalXmlDto Parse(Stream xmlStream)
    {
        XDocument document;

        try
        {
            document = XDocument.Load(xmlStream);
        }
        catch (Exception ex) when (
            ex is System.Xml.XmlException ||
            ex is InvalidOperationException)
        {
            throw new ArgumentException(
                "O arquivo XML da nota fiscal é inválido.", ex);
        }

        var root = document.Root;

        if (root is null || root.Name.LocalName != "NotaFiscal")
        {
            throw new ArgumentException(
                "O XML deve possuir o elemento raiz NotaFiscal.");
        }

        var chaveAcesso = LerTextoObrigatorio(root, "ChaveAcesso");
        var cnpjEmitente = LerTextoObrigatorio(root, "CNPJEmitente");
        var dataEmissaoTexto = LerTextoObrigatorio(root, "DataEmissao");
        var numeroPedido = LerTextoObrigatorio(root, "NumeroPedido");

        if (!DateTime.TryParse(
            dataEmissaoTexto,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var dataEmissao))
        {
            throw new ArgumentException(
                "A data de emissão da nota fiscal é inválida.");
        }

        var itensElemento = root.Element("Itens");

        if (itensElemento is null)
        {
            throw new ArgumentException(
                "O XML deve possuir o elemento Itens.");
        }

        var itens = itensElemento.Elements("Item")
            .Select(item =>
            {
                var codigo = LerTextoObrigatorio(item, "CodigoProduto");
                var quantidadeTexto = LerTextoObrigatorio(item, "Quantidade");
                var precoTexto = LerTextoObrigatorio(item, "Preco");

                if (!decimal.TryParse(
                    quantidadeTexto,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var quantidade) || quantidade <= 0)
                {
                    throw new ArgumentException(
                        $"Quantidade inválida para o produto {codigo}.");
                }

                if (!decimal.TryParse(
                    precoTexto,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var preco) || preco < 0)
                {
                    throw new ArgumentException(
                        $"Preço inválido para o produto {codigo}.");
                }

                return new ItemNotaFiscalXmlDto
                {
                    CodigoProduto = codigo,
                    Quantidade = quantidade,
                    Preco = preco
                };
            })
            .ToList();

        if (itens.Count == 0)
        {
            throw new ArgumentException(
                "A nota fiscal deve possuir pelo menos um item.");
        }

        return new NotaFiscalXmlDto
        {
            ChaveAcesso = chaveAcesso,
            CnpjEmitente = new string(
                cnpjEmitente.Where(char.IsDigit).ToArray()),
            DataEmissao = dataEmissao,
            NumeroPedido = numeroPedido,
            Itens = itens
        };
    }

    private static string LerTextoObrigatorio(
        XElement elemento,
        string nomeElemento)
    {
        var valor = elemento.Element(nomeElemento)?.Value?.Trim();

        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException(
                $"O campo {nomeElemento} é obrigatório.");
        }

        return valor;
    }
}
