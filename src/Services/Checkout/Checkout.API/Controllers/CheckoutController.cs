using Checkout.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Checkout.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICriarPedidoUseCase _criarPedidoUseCase;

    // A MÁGICA DO CLEAN ARCHITECTURE: O Controller agora desconhece o RabbitMQ/MassTransit.
    // Tudo que ele sabe é como receber um JSON e chamar o Caso de Uso.
    public CheckoutController(ICriarPedidoUseCase criarPedidoUseCase)
    {
        _criarPedidoUseCase = criarPedidoUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto request)
    {
        // 1. Controller apenas roteia a execução para o Domínio/Aplicação
        var pedidoId = await _criarPedidoUseCase.ExecutarAsync(request);

        // 2. Retorna a resposta HTTP (202 Accepted) indicando processamento assíncrono
        return Accepted(new { Mensagem = "Pedido registrado com sucesso e em fila de processamento", PedidoId = pedidoId });
    }
}
