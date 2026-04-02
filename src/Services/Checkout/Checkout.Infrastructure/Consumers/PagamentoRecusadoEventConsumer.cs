using MassTransit;
using Microsoft.Extensions.Logging;
using ProcessamentoPedidos.Core.Events;
using Checkout.Application.UseCases;
using System.Threading.Tasks;

namespace Checkout.Infrastructure.Consumers;

public class PagamentoRecusadoEventConsumer : IConsumer<PagamentoRecusadoEvent>
{
    private readonly ILogger<PagamentoRecusadoEventConsumer> _logger;
    private readonly IAtualizarStatusPedidoUseCase _atualizarStatusUseCase;

    public PagamentoRecusadoEventConsumer(
        ILogger<PagamentoRecusadoEventConsumer> logger,
        IAtualizarStatusPedidoUseCase atualizarStatusUseCase)
    {
        _logger = logger;
        _atualizarStatusUseCase = atualizarStatusUseCase;
    }

    public async Task Consume(ConsumeContext<PagamentoRecusadoEvent> context)
    {
        var evento = context.Message;
        _logger.LogInformation($"[CHECKOUT] -> Pagamento Recusado recebido para o Pedido ID: {evento.PedidoId}. Motivo: {evento.MotivoRecusa}");

        await _atualizarStatusUseCase.CancelarAsync(evento.PedidoId);
        
        _logger.LogInformation($"[CHECKOUT] -> Status do Pedido ID: {evento.PedidoId} atualizado para Cancelado.");
    }
}
