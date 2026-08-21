namespace KorpERP.Shared.Contracts.NotaFiscal;

public record NotaFiscalItemFalhou
{
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
    public string MotivoFalha { get; init; } = string.Empty;
}