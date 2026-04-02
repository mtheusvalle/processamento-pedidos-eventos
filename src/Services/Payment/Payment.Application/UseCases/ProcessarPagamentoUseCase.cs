using MassTransit;
using Payment.Domain.Entities;
using Payment.Domain.Interfaces;
using ProcessamentoPedidos.Core.Events;
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
    private readonly IPublishEndpoint _publishEndpoint;

    public ProcessarPagamentoUseCase(IPagamentoRepository pagamentoRepository, IPublishEndpoint publishEndpoint)
    {
        _pagamentoRepository = pagamentoRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task ExecutarAsync(Guid pedidoOriginalId, decimal valorTotal)
    {
        // 1. Cria a entidade com as ricas regras de negócio (nasce Pendente)
        var pagamento = new Pagamento(pedidoOriginalId, valorTotal);

        // Imagine que aqui nós conectaríamos com uma API do Stripe/PayPal
        // Mas por enquanto, vamos auto aprovar só se o valor > 0
        bool aprovado = pagamento.Valor > 0;
        
        if (aprovado)
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

        // 3. Publica evento de retorno para o coreógrafo (Checkout)
        if (aprovado)
        {
            await _publishEndpoint.Publish(new PagamentoAprovadoEvent 
            { 
                PedidoId = pedidoOriginalId, 
                ValorTotal = valorTotal 
            });
        }
        else
        {
            await _publishEndpoint.Publish(new PagamentoRecusadoEvent 
            { 
                PedidoId = pedidoOriginalId, 
                MotivoRecusa = "Saldo Insuficiente ou Cartão Inválido" 
            });
        }
    }
}
