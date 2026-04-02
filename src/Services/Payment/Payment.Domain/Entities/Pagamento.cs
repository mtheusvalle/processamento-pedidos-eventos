using Payment.Domain.Enums;
using System;

namespace Payment.Domain.Entities;

public class Pagamento
{
    public Guid Id { get; private set; }
    public Guid PedidoOriginalId { get; private set; } // Referência para o Checkout, mas sem foreign key rígida!
    public decimal Valor { get; private set; }
    public StatusPagamento Status { get; private set; }
    public DateTime DataProcessamento { get; private set; }

    protected Pagamento() { }

    public Pagamento(Guid pedidoOriginalId, decimal valor)
    {
        Id = Guid.NewGuid();
        PedidoOriginalId = pedidoOriginalId;
        Valor = valor;
        Status = StatusPagamento.Pendente; // Todo pagamento nasce pendente
        DataProcessamento = DateTime.UtcNow;
    }

    public void Aprovar()
    {
        Status = StatusPagamento.Aprovado;
        DataProcessamento = DateTime.UtcNow;
    }

    public void Recusar()
    {
        Status = StatusPagamento.Recusado;
        DataProcessamento = DateTime.UtcNow;
    }
}
