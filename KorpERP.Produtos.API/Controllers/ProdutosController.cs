using Microsoft.AspNetCore.Mvc;
using KorpERP.Produtos.API.DataTransferObjects;
using KorpERP.Produtos.API.Interfaces;
using KorpERP.Produtos.API.Models;

namespace KorpERP.Produtos.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }
    [HttpPost(Name = "CreateProduto")]
    public async Task<ActionResult<ProdutoCriadoDTO>> CreateProduto(ProdutoCriadoDTO produtoDTO)
    {
        try
        {
            var produto = new Produto
            {
                Codigo = produtoDTO.Codigo,
                Descricao = produtoDTO.Descricao,
                Saldo = produtoDTO.SaldoInicial
            };
            var createdProduto = await _produtoService.CreateProdutoAsync(produto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return StatusCode(StatusCodes.Status201Created, produtoDTO);
    }
    [HttpPost("atualizar-produto", Name = "AtualizarProduto")]
    public async Task<ActionResult<ProdutoAtualizadoDTO>> AtualizarProduto(ProdutoAtualizadoDTO produtoAtualizadoDTO)
    {
        try
        {
            await _produtoService.AtualizarProdutoAsync(
            produtoAtualizadoDTO.ProdutoId,
            produtoAtualizadoDTO.NovoSaldo,
            produtoAtualizadoDTO.NovoCodigo,
            produtoAtualizadoDTO.NovoDescricao);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }
}
