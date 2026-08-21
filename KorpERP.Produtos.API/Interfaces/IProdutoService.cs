
using KorpERP.Produtos.API.Models;
using KorpERP.Shared.Contracts.NotaFiscal;

namespace KorpERP.Produtos.API.Interfaces
{
    public interface IProdutoService
    {
        Task<Produto> CreateProdutoAsync(Produto produto);
        Task<Produto> AtualizarProdutoAsync(int produtoId, int novoSaldo, string novoCodigo, string novaDescricao);
        Task<Produto> GetProdutoByIdAsync(int produtoId);
        Task<List<Produto>> GetAllProdutosAsync();
        Task DesativarProdutoAsync(int produtoId);
        Task ProcessarNotaFiscalAsync(int notaFiscalId, List<NotaFiscalItem> itens);
    }
}