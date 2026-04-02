using MassTransit;
using Microsoft.Extensions.Logging;
using ProcessamentoPedidos.Core.Events;
using Checkout.Application.UseCases;
using System.Threading.Tasks;

namespace Checkout.Infrastructure.Consumers;

public class EstoqueRecusadoEventConsumer : IConsumer<EstoqueRecusadoEvent>
{
    private readonly ILogger<EstoqueRecusadoEventConsumer> _logger;
    private readonly IAtualizarStatusPedidoUseCase _atualizarStatusUseCase;

    public EstoqueRecusadoEventConsumer(
        ILogger<EstoqueRecusadoEventConsumer> logger,
        IAtualizarStatusPedidoUseCase atualizarStatusUseCase)
    {
        _logger = logger;
        _atualizarStatusUseCase = atualizarStatusUseCase;
    }

    public async Task Consume(ConsumeContext<EstoqueRecusadoEvent> context)
    {
        var evento = context.Message;
        _logger.LogWarning($"[CHECKOUT] -> Estoque Recusado recebido para o Pedido ID: {evento.PedidoId}. Motivo: {evento.MotivoRecusa}. Cancelando Pedido...");

        await _atualizarStatusUseCase.CancelarAsync(evento.PedidoId);
        
        _logger.LogInformation($"[CHECKOUT] -> Status do Pedido ID: {evento.PedidoId} atualizado para Cancelado devido à falta de estoque.");
    }
}
