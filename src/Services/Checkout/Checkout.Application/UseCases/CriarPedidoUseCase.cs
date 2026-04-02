using Checkout.Domain.Entities;
using Checkout.Domain.Interfaces;
using ProcessamentoPedidos.Core.Events;
using System;
using System.Threading.Tasks;

namespace Checkout.Application.UseCases;

public interface ICriarPedidoUseCase
{
    Task<Guid> ExecutarAsync(CriarPedidoDto dto);
}

public class CriarPedidoUseCase : ICriarPedidoUseCase
{
    private readonly IMessageBusService _messageBusService;
    private readonly IPedidoRepository _pedidoRepository;

    public CriarPedidoUseCase(IMessageBusService messageBusService, IPedidoRepository pedidoRepository)
    {
        _messageBusService = messageBusService;
        _pedidoRepository = pedidoRepository;
    }

    public async Task<Guid> ExecutarAsync(CriarPedidoDto dto)
    {
        // 1. Instanciar entidade do Domínio (Regras Ricas do Negócio)
        var pedido = new Pedido(dto.ClienteCpf, dto.ValorTotal);

        // 2. Salvar no Banco Fisicamente (Isolamento total: pode ser Postgres, SQL Server, MongoDB)
        await _pedidoRepository.AddAsync(pedido);
        await _pedidoRepository.CommitAsync();

        // 3. O 'Id' agora é real pois persistimos.
        // Montar evento de notificação de integração assíncrona.
        var evento = new PedidoCriadoEvent
        {
            PedidoId = pedido.Id,
            ClienteCpf = pedido.ClienteCpf,
            ValorTotal = pedido.ValorTotal,
            DataCriacao = pedido.DataCriacao
        };

        // 4. Dispara a mensagem via RabbitMQ para o resto do ecossistema processar
        await _messageBusService.PublishPedidoCriadoAsync(evento);

        return pedido.Id;
    }
}
