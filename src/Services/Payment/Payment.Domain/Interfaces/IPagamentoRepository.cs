using Payment.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Payment.Domain.Interfaces;

public interface IPagamentoRepository
{
    Task<Pagamento?> GetByPedidoOriginalIdAsync(Guid pedidoOriginalId);
    Task AddAsync(Pagamento pagamento);
    Task CommitAsync();
}
