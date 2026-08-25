namespace KorpERP.Notas.API.Models;

public record NotaFiscalItemProcessado
{
    public int Id { get; init; }
    public Nota Nota { get; init; } = null!;
    public int NotaId { get; init; }
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
    public bool Processado { get; set; } = false;
}