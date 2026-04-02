using Payment.Domain.Entities;
using Payment.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Payment.Application.UseCases;

public interface IProcessarPagamentoUseCase
{
    Task ExecutarAsync(Guid pedidoOriginalId, decimal valorTotal);
}

public class ProcessarPagamentoUseCase : IProcessarPagamentoUseCase
{
    private readonly IPagamentoRepository _pagamentoRepository;

    public ProcessarPagamentoUseCase(IPagamentoRepository pagamentoRepository)
    {
        _pagamentoRepository = pagamentoRepository;
    }

    public async Task ExecutarAsync(Guid pedidoOriginalId, decimal valorTotal)
    {
        // 1. Cria a entidade com as ricas regras de negócio (nasce Pendente)
        var pagamento = new Pagamento(pedidoOriginalId, valorTotal);

        // Imagine que aqui nós conectaríamos com uma API do Stripe/PayPal
        // Mas por enquanto, vamos auto aprovar só se o valor > 0
        if (pagamento.Valor > 0)
        {
            pagamento.Aprovar();
        }
        else
        {
            pagamento.Recusar();
        }

        // 2. Salva no Postgres exclusivo do Worker de Pagamento
        await _pagamentoRepository.AddAsync(pagamento);
        await _pagamentoRepository.CommitAsync();
    }
}
