using KorpERP.Notas.API.Interfaces;
using KorpERP.Notas.API.Persistence;
using KorpERP.Notas.API.Models;
using KorpERP.Shared.Contracts.Produto;
using Microsoft.EntityFrameworkCore;

namespace KorpERP.Notas.API.Services;

public class ProdutoProjectionService : IProdutoProjectionService
{
    private readonly NotasDbContext _context;

    public ProdutoProjectionService(NotasDbContext context)
    {
        _context = context;
    }

    public async Task CreateProdutoProjectionAsync(int produtoId, string codigo = "", string descricao = "")
    {
        var produtoProjection = await _context.Produtos.FindAsync(produtoId);
        if (produtoProjection == null)
        {
            produtoProjection = new ProdutoProjection
            {
                ProdutoProjectionId = produtoId,
                Codigo = codigo,
                Descricao = descricao
            };

            _context.Produtos.Add(produtoProjection);
        }
        else
        {
            produtoProjection.Codigo = codigo;
            produtoProjection.Descricao = descricao;
        }
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarEstoqueAsync(int produtoId, int novoSaldo)
    {
        var produtoProjection = await _context.Produtos.FindAsync(produtoId);
        if (produtoProjection == null)
        {
            _context.Produtos.Add(new ProdutoProjection
            {
                ProdutoProjectionId = produtoId,
                Saldo = novoSaldo
            });
        }
        else
        {
            produtoProjection.Saldo = novoSaldo;
        }
        await _context.SaveChangesAsync();
    }
    public async Task AtualizarProdutoProjectionAsync(int produtoId, string codigo, string descricao)
    {
        var produtoProjection = await _context.Produtos.FindAsync(produtoId);

        if (produtoProjection == null)
        {
            throw new KeyNotFoundException($"ProdutoProjection com ID {produtoId} não encontrado.");
        }
        else
        {
            produtoProjection.Codigo = codigo;
            produtoProjection.Descricao = descricao;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DesativarProdutoProjectionAsync(int produtoId)
    {
        var produtoProjection = await _context.Produtos.FindAsync(produtoId);

        if (produtoProjection == null)
        {
            throw new KeyNotFoundException($"ProdutoProjection com ID {produtoId} não encontrado.");
        }
        else
        {
            produtoProjection.Status = StatusProduto.Inativo;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<ProdutoProjection> GetProdutoProjectionByIdAsync(int produtoId)
    {
        var produtoProjection = await _context.Produtos.FindAsync(produtoId);

        if (produtoProjection == null)
        {
            throw new KeyNotFoundException($"ProdutoProjection com ID {produtoId} não encontrado.");
        }

        return produtoProjection;
    }
    public async Task<List<ProdutoProjection>> GetAllProdutoProjectionsAsync()
    {
        return await _context.Produtos.ToListAsync();
    }
}