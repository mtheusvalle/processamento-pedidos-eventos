using Checkout.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Checkout.Data.Mappings;

/// <summary>
/// Fluent API Mapping. Aqui dizemos EXATAMENTE como a classe Pedido vira uma Tabela (Orders).
/// Essa abstração preserva o Domínio limpo.
/// </summary>
public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        // Nome da tabela principal no banco PostgreSQL
        builder.ToTable("Pedidos");

        // Chave Primária
        builder.HasKey(p => p.Id);

        // O tipo de dados na tabela
        builder.Property(p => p.ClienteCpf)
               .IsRequired()
               .HasMaxLength(14)
               .HasColumnName("cpf_cliente"); // Mapeia a propriedade ClienteCpf para a coluna cpf_cliente
               
        builder.Property(p => p.ValorTotal)
               .IsRequired()
               .HasColumnType("decimal(18,2)"); // Dinheiro precisa de precisão exata

        builder.Property(p => p.DataCriacao)
               .IsRequired();
    }
}
