using KorpERP.Notas.API.Models;
using SharedNotaFiscalItem = KorpERP.Shared.Contracts.NotaFiscal.NotaFiscalItem;
using SharedNotaFiscalItemFalhou = KorpERP.Shared.Contracts.NotaFiscal.NotaFiscalItemFalhou;

namespace KorpERP.Notas.API.Interfaces;

public interface INotasService
{
    Task<Nota> CreateNotaAsync(List<SharedNotaFiscalItem> itens);
    Task<Nota> GetNotaByIdAsync(int notaId);
    Task<List<Nota>> GetAllNotasAsync();
    Task<Nota> AtualizarNotaAsync(int notaId, List<NotaFiscalItem> novosItens);
    Task<Nota> DeletarNotaAsync(int notaId);
    Task<Nota> ProcessarNotaAsync(int notaId);
    Task<Nota> ConcluirNotaAsync(int notaId, List<SharedNotaFiscalItemFalhou> eventItensFalhados, List<SharedNotaFiscalItem> eventItensProcessados);
}