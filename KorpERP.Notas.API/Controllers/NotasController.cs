using Microsoft.AspNetCore.Mvc;
using KorpERP.Notas.API.DataTransferObjects;
using KorpERP.Notas.API.Interfaces;
using KorpERP.Notas.API.Models;

namespace KorpERP.Notas.API.Controllers;

[ApiController]
[Route("[controller]")]
public class NotasController : ControllerBase
{
    private readonly INotasService _notasService;
    private readonly IProdutoProjectionService _produtoProjectionService;

    public NotasController(
        INotasService notasService,
        IProdutoProjectionService produtoProjectionService)
    {
        _notasService = notasService;
        _produtoProjectionService = produtoProjectionService;
    }
    [HttpPost(Name = "CreateNota")]
    public async Task<ActionResult<NotaResponseDTO>> CreateNota(NotaCriadaDTO notaCriadaDTO)
    {
        try
        {
            var notaCriada = await _notasService.CreateNotaAsync(notaCriadaDTO.Itens);
            return StatusCode(StatusCodes.Status201Created, MapToDTO(notaCriada));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut(Name = "AtualizarNota")]
    public async Task<ActionResult<NotaResponseDTO>> AtualizarNota(NotaAtualizadaDTO notaAtualizada)
    {
        try
        {
            var notaAtualizadaResult = await _notasService.AtualizarNotaAsync(notaAtualizada.NotaId, notaAtualizada.Itens);
            return Ok(MapToDTO(notaAtualizadaResult));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{notaId}", Name = "DeletarNota")]
    public async Task<ActionResult<Nota>> DeletarNota(int notaId)
    {
        try
        {
            var notaDeletada = await _notasService.DeletarNotaAsync(notaId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{notaId}", Name = "GetNotaById")]
    public async Task<ActionResult<NotaResponseDTO>> GetNotaById(int notaId)
    {
        try
        {
            var nota = await _notasService.GetNotaByIdAsync(notaId);
            return Ok(MapToDTO(nota));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("todos", Name = "GetAllNotas")]
    public async Task<ActionResult<IEnumerable<NotaResponseDTO>>> GetAllNotas()
    {
        try
        {
            var notas = await _notasService.GetAllNotasAsync();
            return Ok(notas.Select(MapToDTO));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("produtos", Name = "GetAllProdutoProjections")]
    public async Task<ActionResult<IEnumerable<ProdutoProjectionResponseDTO>>> GetAllProdutoProjections()
    {
        try
        {
            var produtos = await _produtoProjectionService.GetAllProdutoProjectionsAsync();
            return Ok(produtos.Select(produto => new ProdutoProjectionResponseDTO
            {
                ProdutoId = produto.ProdutoProjectionId,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo,
                Status = produto.Status
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{notaId}/processar", Name = "ProcessarNota")]
    public async Task<ActionResult<NotaResponseDTO>> ProcessarNota(int notaId)
    {
        try
        {
            var notaProcessada = await _notasService.ProcessarNotaAsync(notaId);
            return Ok(MapToDTO(notaProcessada));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private static NotaResponseDTO MapToDTO(Nota nota)
    {
        return new NotaResponseDTO
        {
            Id = nota.Id,
            DataCriacao = nota.DataCriacao,
            DataFechamento = nota.DataFechamento,
            Status = nota.Status,
            EmProcessamento = nota.EmProcessamento,
            Itens = nota.Itens.Select(item => new NotaFiscalItemResponseDTO
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade
            }).ToList(),
            ItensOk = nota.ItensProcessados.Select(item => new NotaFiscalItemResponseDTO
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade
            }).ToList(),
            ItensFalhados = nota.ItensFalhados.Select(item => new NotaFiscalItemFalhouResponseDTO
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                MotivoFalha = item.MotivoFalha
            }).ToList()
        };
    }
}
