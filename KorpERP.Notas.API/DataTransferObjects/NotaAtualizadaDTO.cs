using KorpERP.Notas.API.Models;

namespace KorpERP.Notas.API.DataTransferObjects
{
    public class NotaAtualizadaDTO
    {
        public int NotaId { get; set; }
        public List<NotaFiscalItem> Itens { get; set; } = new List<NotaFiscalItem>();
    }
}