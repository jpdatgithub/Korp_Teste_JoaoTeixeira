namespace KorpERP.Produtos.API.DataTransferObjects
{
    public class ProdutoAtualizadoDTO
    {
        public int ProdutoId { get; set; }
        public int NovoSaldo { get; set; }
        public string NovoCodigo { get; set; } = string.Empty;
        public string NovoDescricao { get; set; } = string.Empty;
    }
}