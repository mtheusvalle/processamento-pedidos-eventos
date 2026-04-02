using System;
using Checkout.Domain.Entities;
using Checkout.Domain.Interfaces;
using Checkout.Data.Context;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Checkout.Data.Repositories;

/// <summary>
/// O Adapter Secundário de Banco de Dados.
/// Ele pega o contrato do IPedidoRepository (que é C# puro)
/// e injeta nele o AppDbContext (que é acoplado ao Entity Framework).
/// </summary>
public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido> GetByIdAsync(Guid id)
    {
        return await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Pedido pedido)
    {
        // AddAsync prepara as queries INSERT na memória do EntityFramework
        await _context.Pedidos.AddAsync(pedido);
    }

    public async Task CommitAsync()
    {
        // Exige a gravação transacional ("Roda os scripts SQL INSERT pendentes")
        await _context.SaveChangesAsync();
    }
}
