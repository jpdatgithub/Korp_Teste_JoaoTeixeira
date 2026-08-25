using KorpERP.Notas.API.Models;

namespace KorpERP.Notas.API.Interfaces;

public interface IProdutoProjectionService
{
    Task CreateProdutoProjectionAsync(int produtoId, string codigo, string descricao, long versao);
    Task AtualizarEstoqueAsync(int produtoId, int novoSaldo, long versao);
    Task AtualizarProdutoProjectionAsync(int produtoId, string codigo, string descricao, long versao);
    Task DesativarProdutoProjectionAsync(int produtoId, long versao);
    Task<ProdutoProjection> GetProdutoProjectionByIdAsync(int produtoId);
    Task<List<ProdutoProjection>> GetAllProdutoProjectionsAsync();
}