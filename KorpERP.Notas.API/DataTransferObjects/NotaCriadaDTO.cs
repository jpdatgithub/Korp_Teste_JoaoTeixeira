using KorpERP.Shared.Contracts.NotaFiscal;

namespace KorpERP.Notas.API.DataTransferObjects
{
    public class NotaCriadaDTO
    {
        public List<NotaFiscalItem> Itens { get; set; } = new List<NotaFiscalItem>();
    }
}