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
    public NotasController(INotasService notasService)
    {
        _notasService = notasService;
    }
    [HttpPost(Name = "CreateNota")]
    public async Task<ActionResult<NotaCriadaDTO>> CreateNota(NotaCriadaDTO notaCriadaDTO)
    {
        try
        {
            var notaCriada = await _notasService.CreateNotaAsync(notaCriadaDTO.Itens);
            return StatusCode(StatusCodes.Status201Created, notaCriada);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut(Name = "AtualizarNota")]
    public async Task<ActionResult<NotaAtualizadaDTO>> AtualizarNota(NotaAtualizadaDTO notaAtualizada)
    {
        try
        {
            var notaAtualizadaResult = await _notasService.AtualizarNotaAsync(notaAtualizada.NotaId, notaAtualizada.Itens);
            return StatusCode(StatusCodes.Status200OK, notaAtualizadaResult);
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
    public async Task<ActionResult<Nota>> GetNotaById(int notaId)
    {
        try
        {
            var nota = await _notasService.GetNotaByIdAsync(notaId);
            return Ok(nota);
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
    public async Task<ActionResult<IEnumerable<Nota>>> GetAllNotas()
    {
        try
        {
            var notas = await _notasService.GetAllNotasAsync();
            return Ok(notas);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{notaId}/processar", Name = "ProcessarNota")]
    public async Task<ActionResult<Nota>> ProcessarNota(int notaId)
    {
        try
        {
            var notaProcessada = await _notasService.ProcessarNotaAsync(notaId);
            return Ok(notaProcessada);
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
}
