using KorpERP.Shared.Contracts.Produto;

namespace KorpERP.Produtos.API.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Saldo { get; set; } = 0;
        public StatusProduto Status { get; set; } = StatusProduto.Ativo;
    }
}