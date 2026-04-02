using Checkout.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Checkout.Data.Context;

/// <summary>
/// A ponte de conexão real com o PostgreSQL.
/// O DbContext centraliza a sessão de gravação com o banco.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ao invez de espalhar .Property() aqui, mandamos ele varrer o nosso assembly
        // buscando todas as classes que implementam 'IEntityTypeConfiguration' (O Fluent API!).
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}
