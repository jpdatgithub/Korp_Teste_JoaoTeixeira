namespace KorpERP.Notas.API.Models;

public record NotaFiscalItemFalhou
{
    public int Id { get; init; }
    public Nota Nota { get; init; } = new Nota();
    public int NotaId { get; init; }
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
    public string MotivoFalha { get; init; } = string.Empty;
}