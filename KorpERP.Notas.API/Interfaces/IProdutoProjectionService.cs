using KorpERP.Notas.API.Models;

namespace KorpERP.Notas.API.Interfaces;

public interface IProdutoProjectionService
{
    Task CreateProdutoProjectionAsync(int produtoId, string codigo, string descricao);
    Task AtualizarEstoqueAsync(int produtoId, int novoSaldo);
    Task AtualizarProdutoProjectionAsync(int produtoId, string codigo, string descricao);
    Task DesativarProdutoProjectionAsync(int produtoId);
    Task<ProdutoProjection> GetProdutoProjectionByIdAsync(int produtoId);
    Task<List<ProdutoProjection>> GetAllProdutoProjectionsAsync();
}