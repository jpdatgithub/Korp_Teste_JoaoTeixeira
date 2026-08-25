namespace KorpERP.Shared.Events;

public record EstoqueAtualizadoEvent
{
    public int ProdutoId { get; init; }
    public int NovoSaldo { get; init; }
    public long Versao { get; init; }
    public DateTime DataAtualizacao { get; init; }
}
