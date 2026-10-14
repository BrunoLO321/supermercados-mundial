using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class EstoqueConfiguration
    : IEntityTypeConfiguration<Estoque>
{
    public void Configure(
        EntityTypeBuilder<Estoque> builder)
    {
        builder.ToTable("Estoques");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantidade)
            .HasPrecision(18, 3);

        builder.HasIndex(x => x.ProdutoId)
            .IsUnique();
    }
}