using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class ItemPedidoCompraConfiguration
    : IEntityTypeConfiguration<ItemPedidoCompra>
{
    public void Configure(
        EntityTypeBuilder<ItemPedidoCompra> builder)
    {
        builder.ToTable("ItensPedidoCompra");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantidadeSolicitada)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.QuantidadeRecebida)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.PrecoUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(x => x.Produto)
            .WithMany(x => x.ItensPedido)
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}