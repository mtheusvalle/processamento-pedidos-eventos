using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Payment.Application.UseCases;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Interfaces;
using Xunit;

namespace Payment.UnitTests.Application.UseCases;

public class ProcessarPagamentoUseCaseTests
{
    private readonly IPagamentoRepository _pagamentoRepositoryMock;
    private readonly ProcessarPagamentoUseCase _sut;

    public ProcessarPagamentoUseCaseTests()
    {
        _pagamentoRepositoryMock = Substitute.For<IPagamentoRepository>();
        _sut = new ProcessarPagamentoUseCase(_pagamentoRepositoryMock);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoValorMaiorQueZero_DeveSalvarComoAprovado()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var valorTotal = 100.00m;

        // Act
        await _sut.ExecutarAsync(pedidoId, valorTotal);

        // Assert
        await _pagamentoRepositoryMock.Received(1).AddAsync(Arg.Is<Pagamento>(p => 
            p.PedidoOriginalId == pedidoId && 
            p.Valor == valorTotal && 
            p.Status == StatusPagamento.Aprovado)); // Regra principal
        
        await _pagamentoRepositoryMock.Received(1).CommitAsync();
    }

    [Fact]
    public async Task ExecutarAsync_QuandoValorForZeroOuMenor_DeveSalvarComoRecusado()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var valorTotal = 0.00m; // Vai falhar na aprovação

        // Act
        await _sut.ExecutarAsync(pedidoId, valorTotal);

        // Assert
        await _pagamentoRepositoryMock.Received(1).AddAsync(Arg.Is<Pagamento>(p => 
            p.PedidoOriginalId == pedidoId && 
            p.Status == StatusPagamento.Recusado)); // Regra de Recusa
        
        await _pagamentoRepositoryMock.Received(1).CommitAsync();
    }
}
