using Payment.Domain.Interfaces;
using Payment.Domain.Enums;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Payment.Application.UseCases;

public interface IEstornarPagamentoUseCase
{
    Task ExecutarAsync(Guid pedidoOriginalId);
}

public class EstornarPagamentoUseCase : IEstornarPagamentoUseCase
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly ILogger<EstornarPagamentoUseCase> _logger;

    public EstornarPagamentoUseCase(IPagamentoRepository pagamentoRepository, ILogger<EstornarPagamentoUseCase> logger)
    {
        _pagamentoRepository = pagamentoRepository;
        _logger = logger;
    }

    public async Task ExecutarAsync(Guid pedidoOriginalId)
    {
        var pagamento = await _pagamentoRepository.GetByPedidoOriginalIdAsync(pedidoOriginalId);
        
        if (pagamento == null)
        {
            _logger.LogWarning($"Pagamento para o pedido {pedidoOriginalId} não encontrado. Não é possível estornar.");
            return;
        }

        if (pagamento.Status == StatusPagamento.Aprovado)
        {
            pagamento.Estornar();
            await _pagamentoRepository.CommitAsync();
            _logger.LogInformation($"Pagamento {pagamento.Id} referente ao pedido {pedidoOriginalId} foi estornado com sucesso.");
        }
        else
        {
            _logger.LogInformation($"Pagamento {pagamento.Id} não estava aprovado (Status: {pagamento.Status}), nenhum estorno necessário.");
        }
    }
}
