namespace KorpERP.Produtos.API.DataTransferObjects
{
    public class ProdutoCriadoDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int SaldoInicial { get; set; } = 0;
    }
}