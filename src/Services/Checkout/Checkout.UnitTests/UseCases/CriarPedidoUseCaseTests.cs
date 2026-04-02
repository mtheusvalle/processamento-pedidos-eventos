using System;
using System.Threading.Tasks;
using Checkout.Application.UseCases;
using Checkout.Domain.Entities;
using Checkout.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using ProcessamentoPedidos.Core.Events;
using Xunit;

namespace Checkout.UnitTests.Application.UseCases;

public class CriarPedidoUseCaseTests
{
    private readonly IMessageBusService _messageBusServiceMock;
    private readonly IPedidoRepository _pedidoRepositoryMock;
    private readonly CriarPedidoUseCase _sut; // System Under Test

    public CriarPedidoUseCaseTests()
    {
        // 1. Arrange global: Criamos nossos Dublês (Mocks)
        _messageBusServiceMock = Substitute.For<IMessageBusService>();
        _pedidoRepositoryMock = Substitute.For<IPedidoRepository>();

        _sut = new CriarPedidoUseCase(_messageBusServiceMock, _pedidoRepositoryMock);
    }

    [Fact]
    public async Task ExecutarAsync_DeveSalvarPedidoPublicarEvento_ERetornarId()
    {
        // Arrange
        var dto = new CriarPedidoDto("12345678900", 150.50m);

        // Act
        var result = await _sut.ExecutarAsync(dto);

        // Assert
        // Garante que retornou um Guid válido
        result.Should().NotBeEmpty();

        // Garante que o Repositório persistiu o objeto de Domínio corretamente
        await _pedidoRepositoryMock.Received(1).AddAsync(Arg.Is<Pedido>(p => 
            p.ClienteCpf == dto.ClienteCpf && p.ValorTotal == dto.ValorTotal));
        
        await _pedidoRepositoryMock.Received(1).CommitAsync();

        // Garante que a integração (Fila) publicou o evento
        await _messageBusServiceMock.Received(1).PublishPedidoCriadoAsync(Arg.Is<PedidoCriadoEvent>(e => 
            e.PedidoId == result && e.ValorTotal == dto.ValorTotal));
    }
}
