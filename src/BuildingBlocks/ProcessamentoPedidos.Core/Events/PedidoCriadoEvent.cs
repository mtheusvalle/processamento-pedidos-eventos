namespace ProcessamentoPedidos.Core.Events;

/// <summary>
/// Contrato de Evento que representa a criação de um novo pedido.
/// Será trafegado no Message Broker (RabbitMQ/SQS).
/// Como boa prática de Clean Architecture e de Integração, eventos devem ser simples (apenas dados, sem comportamento).
/// </summary>
public record PedidoCriadoEvent
{
    public Guid PedidoId { get; init; }
    public string ClienteCpf { get; init; } = string.Empty;
    public decimal ValorTotal { get; init; }
    public DateTime DataCriacao { get; init; } = DateTime.UtcNow;
}
