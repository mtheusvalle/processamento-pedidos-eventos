using System;

namespace ProcessamentoPedidos.Core.Events;

public record PagamentoAprovadoEvent
{
    public Guid PedidoId { get; init; }
    public decimal ValorTotal { get; init; }
    public DateTime DataProcessamento { get; init; } = DateTime.UtcNow;
}
