using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class AcordoComercialConfiguration
    : IEntityTypeConfiguration<AcordoComercial>
{
    public void Configure(
        EntityTypeBuilder<AcordoComercial> builder)
    {
        builder.ToTable("AcordosComerciais");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ValorDivergencia)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Motivo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.DataCriacao)
            .IsRequired();

        builder.HasOne(x => x.PedidoCompra)
            .WithMany()
            .HasForeignKey(x => x.PedidoCompraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}