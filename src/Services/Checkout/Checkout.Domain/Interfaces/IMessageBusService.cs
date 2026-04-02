using ProcessamentoPedidos.Core.Events;
using System.Threading.Tasks;

namespace Checkout.Domain.Interfaces;

/// <summary>
/// Porta (Port/Interface) de Saída no padrão Clean Architecture.
/// O domínio sabe que precisa "Publicar" mensagens, mas não faz ideia de COMO (MassTransit) ou ONDE (RabbitMQ/SQS).
/// </summary>
public interface IMessageBusService
{
    Task PublishPedidoCriadoAsync(PedidoCriadoEvent evento);
}
