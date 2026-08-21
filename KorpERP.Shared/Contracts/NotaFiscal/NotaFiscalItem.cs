namespace KorpERP.Shared.Contracts.NotaFiscal;

public record NotaFiscalItem
{
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
}