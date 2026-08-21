namespace KorpERP.Notas.API.Models;

public record NotaFiscalItem
{
    public Nota Nota { get; init; } = null!;
    public int NotaId { get; init; }
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
}