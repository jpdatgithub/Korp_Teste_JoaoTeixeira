using KorpERP.Shared.Contracts.Produto;

namespace KorpERP.Notas.API.DataTransferObjects;

public class ProdutoProjectionResponseDTO
{
    public int ProdutoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
    public StatusProduto Status { get; set; }
}