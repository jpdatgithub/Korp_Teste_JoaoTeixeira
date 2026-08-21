namespace KorpERP.Notas.API.DataTransferObjects
{
    public class NotaFiscalItemFalhouResponseDTO
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public string MotivoFalha { get; set; } = string.Empty;
    }
}