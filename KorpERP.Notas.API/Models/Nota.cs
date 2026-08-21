using KorpERP.Shared.Contracts.NotaFiscal;

namespace KorpERP.Notas.API.Models;

public class Nota
{
    public int Id { get; set; }
    public List<NotaFiscalItem> Itens { get; set; } = new List<NotaFiscalItem>();
    public List<NotaFiscalItem> ItensProcessados { get; set; } = new List<NotaFiscalItem>();
    public List<NotaFiscalItemFalhou> ItensFalhados { get; set; } = new List<NotaFiscalItemFalhou>();
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataFechamento { get; set; }
    public StatusNota Status { get; set; } = StatusNota.Aberta;
    public bool EmProcessamento { get; set; } = false;
}