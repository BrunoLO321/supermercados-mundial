using Microsoft.EntityFrameworkCore;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence;

public class MundialDbContext : DbContext
{
    public MundialDbContext(
        DbContextOptions<MundialDbContext> options)
        : base(options)
    {
    }

    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    public DbSet<Produto> Produtos => Set<Produto>();

    public DbSet<Estoque> Estoques => Set<Estoque>();

    public DbSet<PedidoCompra> PedidosCompra => Set<PedidoCompra>();

    public DbSet<ItemPedidoCompra> ItensPedidoCompra => Set<ItemPedidoCompra>();

    public DbSet<DocumentoFiscal> DocumentosFiscais => Set<DocumentoFiscal>();

    public DbSet<ItemDocumentoFiscal> ItensDocumentoFiscal => Set<ItemDocumentoFiscal>();

    public DbSet<AcordoComercial> AcordosComerciais => Set<AcordoComercial>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MundialDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}