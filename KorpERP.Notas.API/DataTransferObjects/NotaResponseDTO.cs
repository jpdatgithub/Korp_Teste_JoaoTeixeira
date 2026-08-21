using KorpERP.Notas.API.Models;
using SharedStatusNota = KorpERP.Shared.Contracts.NotaFiscal.StatusNota;

namespace KorpERP.Notas.API.DataTransferObjects;

public class NotaResponseDTO
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFechamento { get; set; }
    public SharedStatusNota Status { get; set; }
    public bool EmProcessamento { get; set; }
    public List<NotaFiscalItemResponseDTO> Itens { get; set; } = new List<NotaFiscalItemResponseDTO>();
    public List<NotaFiscalItemResponseDTO> ItensOk { get; set; } = new List<NotaFiscalItemResponseDTO>();
    public List<NotaFiscalItemFalhouResponseDTO> ItensFalhados { get; set; } = new List<NotaFiscalItemFalhouResponseDTO>();
}