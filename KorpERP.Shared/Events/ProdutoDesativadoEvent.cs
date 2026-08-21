namespace KorpERP.Shared.Events;

public record ProdutoDesativadoEvent
{
    public int ProdutoId { get; init; }
    public DateTime DataDesativacao { get; init; }
}