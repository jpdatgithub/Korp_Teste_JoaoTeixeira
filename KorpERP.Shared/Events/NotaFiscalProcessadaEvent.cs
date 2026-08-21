using KorpERP.Shared.Contracts.NotaFiscal;

namespace KorpERP.Shared.Events;

public record NotaFiscalProcessadaEvent
{
    public int NotaFiscalId { get; init; }
    public IReadOnlyList<NotaFiscalItem> Itens { get; init; } = [];
}