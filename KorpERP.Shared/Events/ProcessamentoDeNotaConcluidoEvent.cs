using KorpERP.Shared.Contracts.NotaFiscal;

namespace KorpERP.Shared.Events;

public record ProcessamentoDeNotaConcluidoEvent
{
    public int NotaFiscalId { get; init; }
    public IReadOnlyList<NotaFiscalItem> Itens { get; init; } = [];
    public IReadOnlyList<NotaFiscalItemFalhou> ItensFalhos { get; init; } = [];
}