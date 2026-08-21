using Microsoft.AspNetCore.Mvc;
using KorpERP.Notas.API.DataTransferObjects;

namespace KorpERP.Notas.API.Controllers;

[ApiController]
[Route("[controller]")]
public class NotasController : ControllerBase
{
    [HttpPost(Name = "CreateNota")]
    public Task<ActionResult<NotaCriadaDTO>> CreateNota([FromBody] NotaCriadaDTO notaCriada)
    {
        return Task.FromResult<ActionResult<NotaCriadaDTO>>(Ok(notaCriada));
    }
    [HttpPut(Name = "AtualizarNota")]
    public Task<ActionResult<NotaAtualizadaDTO>> AtualizarNota([FromBody] NotaAtualizadaDTO notaAtualizada)
    {
        return Task.FromResult<ActionResult<NotaAtualizadaDTO>>(Ok(notaAtualizada));
    }
}
