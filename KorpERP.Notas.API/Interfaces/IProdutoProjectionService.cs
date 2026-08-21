namespace KorpERP.Notas.API.Interfaces;

public interface IProdutoProjectionService
{
    Task CreateProdutoProjectionAsync(int produtoId, string codigo, string descricao);
    Task AtualizarEstoqueAsync(int produtoId, int novoSaldo);
    Task AtualizarProdutoProjectionAsync(int produtoId, string codigo, string descricao);
    Task DesativarProdutoProjectionAsync(int produtoId);
}