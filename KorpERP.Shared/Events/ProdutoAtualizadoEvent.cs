namespace KorpERP.Shared.Events;

public record ProdutoAtualizadoEvent
{
    public int ProdutoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public DateTime DataAtualizacao { get; init; }
}