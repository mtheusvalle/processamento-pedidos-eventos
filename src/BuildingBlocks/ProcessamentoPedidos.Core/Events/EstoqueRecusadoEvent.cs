using System;

namespace ProcessamentoPedidos.Core.Events;

public record EstoqueRecusadoEvent
{
    public Guid PedidoId { get; init; }
    public string MotivoRecusa { get; init; } = string.Empty;
}
