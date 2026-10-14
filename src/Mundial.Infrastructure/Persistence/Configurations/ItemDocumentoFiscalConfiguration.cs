using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class ItemDocumentoFiscalConfiguration
    : IEntityTypeConfiguration<ItemDocumentoFiscal>
{
    public void Configure(
        EntityTypeBuilder<ItemDocumentoFiscal> builder)
    {
        builder.ToTable("ItensDocumentoFiscal");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantidade)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.PrecoUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(x => x.Produto)
            .WithMany(x => x.ItensDocumentoFiscal)
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}