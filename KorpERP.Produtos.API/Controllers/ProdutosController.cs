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

    [HttpGet("{id}", Name = "GetProdutoById")]
    public async Task<ActionResult<ProdutoResponseDTO>> GetProdutoById(int id)
    {
        try
        {
            var produto = await _produtoService.GetProdutoByIdAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            var produtoResponseDTO = new ProdutoResponseDTO
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo
            };
            return Ok(produtoResponseDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("todos", Name = "GetAllProdutos")]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> GetAllProdutos()
    {
        try
        {
            var produtos = await _produtoService.GetAllProdutosAsync();

            var produtosResponseDTO = produtos.Select(produto => new ProdutoResponseDTO
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo
            }).ToList();

            return Ok(produtosResponseDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("atualizar-produto", Name = "AtualizarProduto")]
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

    [HttpPost("desativar-produto/{id}", Name = "DesativarProduto")]
    public async Task<ActionResult<int>> DesativarProduto(int id)
    {
        try
        {
            await _produtoService.DesativarProdutoAsync(id);
            return Ok(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
