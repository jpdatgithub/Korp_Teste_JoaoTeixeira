namespace KorpERP.Notas.API.Models;

public record NotaFiscalItem
{
    public Nota Nota { get; init; } = new Nota();
    public int NotaId { get; init; }
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
}