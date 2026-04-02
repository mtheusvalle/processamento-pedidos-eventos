using MassTransit;
using Microsoft.Extensions.Logging;
using ProcessamentoPedidos.Core.Events;
using Checkout.Application.UseCases;
using System.Threading.Tasks;

namespace Checkout.Infrastructure.Consumers;

public class PagamentoAprovadoEventConsumer : IConsumer<PagamentoAprovadoEvent>
{
    private readonly ILogger<PagamentoAprovadoEventConsumer> _logger;
    private readonly IAtualizarStatusPedidoUseCase _atualizarStatusUseCase;

    public PagamentoAprovadoEventConsumer(
        ILogger<PagamentoAprovadoEventConsumer> logger,
        IAtualizarStatusPedidoUseCase atualizarStatusUseCase)
    {
        _logger = logger;
        _atualizarStatusUseCase = atualizarStatusUseCase;
    }

    public async Task Consume(ConsumeContext<PagamentoAprovadoEvent> context)
    {
        var evento = context.Message;
        _logger.LogInformation($"[CHECKOUT] -> Pagamento Aprovado recebido para o Pedido ID: {evento.PedidoId}");

        await _atualizarStatusUseCase.MarcarComoPagoAsync(evento.PedidoId);
        
        _logger.LogInformation($"[CHECKOUT] -> Status do Pedido ID: {evento.PedidoId} atualizado para Pago.");
    }
}
