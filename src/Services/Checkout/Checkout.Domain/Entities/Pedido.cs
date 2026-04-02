using System;

namespace Checkout.Domain.Entities;

/// <summary>
/// A Entidade principal rica. 
/// Pura, apenas com C#, lidando com suas próprias regras e estado interno.
/// Não existe a palavra "PostgreSQL" ou "Entity Framework" aqui.
/// </summary>
public class Pedido
{
    public Guid Id { get; private set; }
    public string ClienteCpf { get; private set; }
    public decimal ValorTotal { get; private set; }
    public DateTime DataCriacao { get; private set; }

    // Construtor EF Core (precisa de um construtor vazio, mas mantemos protected para encapsulamento)
    protected Pedido() { }

    public Pedido(string clienteCpf, decimal valorTotal)
    {
        Id = Guid.NewGuid(); // Na vida real, o banco ou um algoritmo como NewId gerenciaria isso
        ClienteCpf = clienteCpf ?? throw new ArgumentNullException(nameof(clienteCpf));
        ValorTotal = valorTotal;
        DataCriacao = DateTime.UtcNow;
    }
}
