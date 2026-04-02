using MassTransit;
using Microsoft.Extensions.Logging;
using ProcessamentoPedidos.Core.Events;
using System;
using System.Threading.Tasks;

namespace Inventory.Worker.Consumers;

public class PedidoCriadoEventConsumer : IConsumer<PedidoCriadoEvent>
{
    private readonly ILogger<PedidoCriadoEventConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public PedidoCriadoEventConsumer(ILogger<PedidoCriadoEventConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<PedidoCriadoEvent> context)
    {
        var evento = context.Message;
        _logger.LogInformation($"[INVENTORY WORKER] -> Analisando estoque para o Pedido ID: {evento.PedidoId}");

        // Simulação de verificação de estoque. 
        // 20% de chance de não ter estoque para fins de teste.
        var random = new Random();
        bool temEstoque = random.Next(1, 100) > 20;

        if (temEstoque)
        {
            _logger.LogInformation($"[INVENTORY WORKER] -> Estoque reservado com sucesso para o Pedido ID: {evento.PedidoId}");
            // Em uma saga completa poderíamos publicar EstoqueReservadoEvent.
        }
        else
        {
            _logger.LogWarning($"[INVENTORY WORKER] -> Estoque insulficiente para o Pedido ID: {evento.PedidoId}");
            
            // Publica o evento de compensação
            await _publishEndpoint.Publish(new EstoqueRecusadoEvent
            {
                PedidoId = evento.PedidoId,
                MotivoRecusa = "Produto fora de estoque"
            });
        }
    }
}
