using Checkout.Domain.Interfaces;
using MassTransit;
using ProcessamentoPedidos.Core.Events;
using System.Threading.Tasks;

namespace Checkout.Infrastructure.Services;

/// <summary>
/// Adaptador Secundário. Implementa a interface exigida pelo Domínio (IMessageBusService),
/// mas usa os pacotes REST/AMQP pesados do MassTransit por baixo dos panos.
/// </summary>
public class MassTransitMessageBusService : IMessageBusService
{
    private readonly IPublishEndpoint _publishEndpoint;

    // A infraestrutura PODE depender livremente do MassTransit
    public MassTransitMessageBusService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishPedidoCriadoAsync(PedidoCriadoEvent evento)
    {
        // Envia para o RabbitTQ
        await _publishEndpoint.Publish(evento);
    }
}
