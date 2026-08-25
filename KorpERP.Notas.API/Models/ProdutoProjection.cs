using KorpERP.Shared.Contracts.Produto;

namespace KorpERP.Notas.API.Models;

public class ProdutoProjection
{
    public int ProdutoProjectionId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; } = 0;
    public StatusProduto Status { get; set; } = StatusProduto.Ativo;
}