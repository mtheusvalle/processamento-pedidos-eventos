using Checkout.Domain.Entities;
using System.Threading.Tasks;

namespace Checkout.Domain.Interfaces;

/// <summary>
/// Interface de Saída.
/// É assim que o 'Application UseCase' salvará o Pedido.
/// </summary>
public interface IPedidoRepository
{
    Task AddAsync(Pedido pedido);
    Task CommitAsync();
}
