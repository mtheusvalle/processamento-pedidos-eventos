using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Data.Mappings;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamentos");

        builder.HasKey(p => p.Id);

        // PedidoOriginalId não é uma chave estrangeira de verdade aqui!
        // Porque a Tabela Pedidos mora no banco Postgres do Checkout! (5432)
        // Isso é uma soft-link. Guardamos apenas o GUID para rastreio do Sistema.
        builder.Property(p => p.PedidoOriginalId)
               .IsRequired()
               .HasColumnName("pedido_original_id");

        builder.Property(p => p.Valor)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        // O enum se salva como numero inteiro 
        builder.Property(p => p.Status)
               .IsRequired()
               .HasConversion<int>()
               .HasColumnName("status_pagamento");

        builder.Property(p => p.DataProcessamento)
               .IsRequired();
    }
}
