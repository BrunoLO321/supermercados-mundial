namespace Mundial.Domain.Entities;

public class Estoque
{
    public int Id { get; private set; }

    public int ProdutoId { get; private set; }

    public Produto Produto { get; private set; } = null!;

    public decimal Quantidade { get; private set; }

    private Estoque()
    {
    }

    public Estoque(Produto produto)
    {
        Produto = produto;
        Quantidade = 0;
    }

    public void Adicionar(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException(
                "A quantidade deve ser maior que zero.");

        Quantidade += quantidade;
    }
}