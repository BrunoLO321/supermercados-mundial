namespace Mundial.Domain.Entities;

public class Fornecedor
{
    public int Id { get; private set; }

    public string Cnpj { get; private set; } = string.Empty;

    public string RazaoSocial { get; private set; } = string.Empty;

    public bool Ativo { get; private set; }

    public ICollection<PedidoCompra> Pedidos { get; private set; }
        = new List<PedidoCompra>();

    private Fornecedor()
    {
    }

    public Fornecedor(
        string cnpj,
        string razaoSocial)
    {
        Cnpj = cnpj;
        RazaoSocial = razaoSocial;
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public void Ativar()
    {
        Ativo = true;
    }
}