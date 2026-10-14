using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundial.Domain.Entities;

namespace Mundial.Infrastructure.Persistence.Configurations;

public class FornecedorConfiguration
    : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(
        EntityTypeBuilder<Fornecedor> builder)
    {
        builder.ToTable("Fornecedores");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Cnpj)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(x => x.Cnpj)
            .IsUnique();

        builder.Property(x => x.RazaoSocial)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Ativo)
            .IsRequired();
    }
}