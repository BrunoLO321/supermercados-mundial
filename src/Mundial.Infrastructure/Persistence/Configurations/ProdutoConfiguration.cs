using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration
    : IEntityTypeConfiguration<Produto>
{
    public void Configure(
        EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Codigo)
            .IsUnique();

        builder.Property(x => x.Descricao)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Preco)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.Estoque)
            .WithOne(x => x.Produto)
            .HasForeignKey<Estoque>(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}