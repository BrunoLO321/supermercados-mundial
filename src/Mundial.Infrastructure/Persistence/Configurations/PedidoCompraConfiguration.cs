using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class PedidoCompraConfiguration
    : IEntityTypeConfiguration<PedidoCompra>
{
    public void Configure(
        EntityTypeBuilder<PedidoCompra> builder)
    {
        builder.ToTable("PedidosCompra");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Numero)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Numero)
            .IsUnique();

        builder.Property(x => x.DataEmissao)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.Fornecedor)
            .WithMany(x => x.Pedidos)
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.PedidoCompra)
            .HasForeignKey(x => x.PedidoCompraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}