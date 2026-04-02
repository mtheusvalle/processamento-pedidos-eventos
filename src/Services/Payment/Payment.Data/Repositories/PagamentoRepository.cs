using Microsoft.EntityFrameworkCore;
using Payment.Data.Context;
using Payment.Domain.Entities;
using Payment.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Payment.Data.Repositories;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly PaymentDbContext _context;

    public PagamentoRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Pagamento?> GetByPedidoOriginalIdAsync(Guid pedidoOriginalId)
    {
        return await _context.Pagamentos.FirstOrDefaultAsync(p => p.PedidoOriginalId == pedidoOriginalId);
    }

    public async Task AddAsync(Pagamento pagamento)
    {
        await _context.Pagamentos.AddAsync(pagamento);
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}
