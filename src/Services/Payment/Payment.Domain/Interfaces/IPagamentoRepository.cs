using Payment.Domain.Entities;
using System.Threading.Tasks;

namespace Payment.Domain.Interfaces;

public interface IPagamentoRepository
{
    Task AddAsync(Pagamento pagamento);
    Task CommitAsync();
}
