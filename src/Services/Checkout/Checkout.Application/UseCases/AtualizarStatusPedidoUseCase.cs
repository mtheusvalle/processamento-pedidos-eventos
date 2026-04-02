using System;
using System.Threading.Tasks;
using Checkout.Domain.Interfaces;
using Checkout.Domain.Enums;

namespace Checkout.Application.UseCases;

public interface IAtualizarStatusPedidoUseCase
{
    Task MarcarComoPagoAsync(Guid id);
    Task CancelarAsync(Guid id);
}

public class AtualizarStatusPedidoUseCase : IAtualizarStatusPedidoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public AtualizarStatusPedidoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task MarcarComoPagoAsync(Guid id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        if (pedido == null) return; // Ou lançar exceção

        pedido.MarcarComoPago();
        await _pedidoRepository.CommitAsync();
    }

    public async Task CancelarAsync(Guid id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        if (pedido == null) return; // Ou lançar exceção

        pedido.Cancelar();
        await _pedidoRepository.CommitAsync();
    }
}
