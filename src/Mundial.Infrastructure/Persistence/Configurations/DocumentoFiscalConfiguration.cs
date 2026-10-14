using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class DocumentoFiscalConfiguration
    : IEntityTypeConfiguration<DocumentoFiscal>
{
    public void Configure(
        EntityTypeBuilder<DocumentoFiscal> builder)
    {
        builder.ToTable("DocumentosFiscais");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChaveAcesso)
            .IsRequired()
            .HasMaxLength(44);

        builder.HasIndex(x => x.ChaveAcesso)
            .IsUnique();

        builder.Property(x => x.CnpjEmitente)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(x => x.DataEmissao)
            .IsRequired();

        builder.HasOne(x => x.PedidoCompra)
            .WithMany(x => x.DocumentosFiscais)
            .HasForeignKey(x => x.PedidoCompraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.DocumentoFiscal)
            .HasForeignKey(x => x.DocumentoFiscalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}