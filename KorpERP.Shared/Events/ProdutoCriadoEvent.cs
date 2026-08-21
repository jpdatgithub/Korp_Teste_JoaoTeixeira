namespace KorpERP.Shared.Events;

public record ProdutoCriadoEvent
{
    public int ProdutoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public DateTime DataCriacao { get; init; }
}