using MassTransit;
using Microsoft.Extensions.Logging;
using ProcessamentoPedidos.Core.Events;
using Payment.Application.UseCases;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Consumers;

public class EstoqueRecusadoEventConsumer : IConsumer<EstoqueRecusadoEvent>
{
    private readonly ILogger<EstoqueRecusadoEventConsumer> _logger;
    private readonly IEstornarPagamentoUseCase _estornarPagamentoUseCase;

    public EstoqueRecusadoEventConsumer(
        ILogger<EstoqueRecusadoEventConsumer> logger,
        IEstornarPagamentoUseCase estornarPagamentoUseCase)
    {
        _logger = logger;
        _estornarPagamentoUseCase = estornarPagamentoUseCase;
    }

    public async Task Consume(ConsumeContext<EstoqueRecusadoEvent> context)
    {
        var evento = context.Message;
        _logger.LogWarning($"[PAYMENT WORKER] -> Estoque Recusado recebido para o Pedido ID: {evento.PedidoId}. Iniciando estorno de pagamento se necessário...");

        await _estornarPagamentoUseCase.ExecutarAsync(evento.PedidoId);
        
        _logger.LogInformation($"[PAYMENT WORKER] -> Processo de estorno concluído para o Pedido ID: {evento.PedidoId}.");
    }
}
