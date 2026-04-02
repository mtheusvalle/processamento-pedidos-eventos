using System;

namespace ProcessamentoPedidos.Core.Events;

public record PagamentoRecusadoEvent
{
    public Guid PedidoId { get; init; }
    public string MotivoRecusa { get; init; } = string.Empty;
    public DateTime DataProcessamento { get; init; } = DateTime.UtcNow;
}
