using MassTransit;
using Microsoft.Extensions.Logging;
using ProcessamentoPedidos.Core.Events;
using Payment.Application.UseCases;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Consumers;

/// <summary>
/// Worker/Consumidor que assina a fila do RabbitMQ aguardando pedidos novos!
/// Ele vive na Infraestrutura pois depende diretamente da assinatura 'IConsumer' do MassTransit.
/// </summary>
public class PedidoCriadoEventConsumer : IConsumer<PedidoCriadoEvent>
{
    private readonly ILogger<PedidoCriadoEventConsumer> _logger;
    private readonly IProcessarPagamentoUseCase _processarPagamentoUseCase;

    public PedidoCriadoEventConsumer(
        ILogger<PedidoCriadoEventConsumer> logger,
        IProcessarPagamentoUseCase processarPagamentoUseCase)
    {
        _logger = logger;
        _processarPagamentoUseCase = processarPagamentoUseCase;
    }

    public async Task Consume(ConsumeContext<PedidoCriadoEvent> context)
    {
        var evento = context.Message;
        _logger.LogInformation("================================================");
        _logger.LogInformation($"[WORKER DE PAGAMENTO] -> Mensagem Capturada!");
        _logger.LogInformation($"Pedido ID: {evento.PedidoId} | Valor: {evento.ValorTotal:C}");
        _logger.LogInformation("================================================");

        // A MÁGICA: O MassTransit (Infra) manda os dados cru para o Application (UseCase limpo)
        await _processarPagamentoUseCase.ExecutarAsync(evento.PedidoId, evento.ValorTotal);
        
        _logger.LogInformation($"[WORKER DE PAGAMENTO] -> Sucesso. Pagamento salvo no Banco de Dados!");
    }
}
