using Microsoft.EntityFrameworkCore;
using KorpERP.Notas.API.Persistence;
using KorpERP.Notas.API.Models;
using KorpERP.Notas.API.Interfaces;
using KorpERP.Shared.Events;
using SharedStatusNota = KorpERP.Shared.Contracts.NotaFiscal.StatusNota;
using SharedNotaFiscalItem = KorpERP.Shared.Contracts.NotaFiscal.NotaFiscalItem;
using SharedNotaFiscalItemFalhou = KorpERP.Shared.Contracts.NotaFiscal.NotaFiscalItemFalhou;

namespace KorpERP.Notas.API.Services;

public class NotasService : INotasService
{
    private readonly NotasDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public NotasService(NotasDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<Nota> CreateNotaAsync(List<SharedNotaFiscalItem> itens)
    {
        var nota = new Nota { };

        foreach (var item in itens)
        {
            var notaFiscalItem = new NotaFiscalItem
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                Nota = nota
                //supostamente deve preencher o notaId automaticamente com base na Nota quando salvar
            };
            nota.Itens.Add(notaFiscalItem);
        }

        _context.Notas.Add(nota);
        await _context.SaveChangesAsync();

        return nota;
    }
    public async Task<Nota> GetNotaByIdAsync(int notaId)
    {
        var nota = await _context.Notas
            .FirstOrDefaultAsync(n => n.Id == notaId);

        if (nota == null)
        {
            throw new KeyNotFoundException($"Nota com ID {notaId} não encontrada.");
        }

        if (nota.Status == SharedStatusNota.Fechada && !nota.EmProcessamento)
        {
            await _context.Entry(nota)
                .Collection(n => n.ItensProcessados)
                .LoadAsync();

            await _context.Entry(nota)
                .Collection(n => n.ItensFalhados)
                .LoadAsync();
        }
        else
        {
            await _context.Entry(nota)
                .Collection(n => n.Itens)
                .LoadAsync();
        }

        return nota;
    }
    public async Task<List<Nota>> GetAllNotasAsync()
    {
        return await _context.Notas
            .Include(n => n.Itens)
            .Include(n => n.ItensProcessados)
            .Include(n => n.ItensFalhados)
            .ToListAsync();
    }
    public async Task<Nota> AtualizarNotaAsync(int notaId, List<NotaFiscalItem> novosItens)
    {
        var nota = await _context.Notas
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == notaId);

        if (nota == null)
        {
            throw new KeyNotFoundException($"Nota com ID {notaId} não encontrada.");
        }

        if (nota.Status == SharedStatusNota.Fechada)
        {
            throw new InvalidOperationException($"A nota {notaId} já está fechada e não pode ser atualizada.");
        }

        nota.Itens = novosItens;

        await _context.SaveChangesAsync();

        return nota;
    }
    public async Task<Nota> DeletarNotaAsync(int notaId)
    {
        var nota = await _context.Notas.FindAsync(notaId);

        if (nota == null)
        {
            throw new KeyNotFoundException($"Nota com ID {notaId} não encontrada.");
        }

        if (nota.Status == SharedStatusNota.Fechada)
        {
            throw new InvalidOperationException($"A nota {notaId} já está fechada e não pode deletada.");
        }

        _context.Notas.Remove(nota);
        await _context.SaveChangesAsync();

        return nota;
    }
    public async Task<Nota> ProcessarNotaAsync(int notaId)
    {
        var nota = await _context.Notas
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == notaId);

        if (nota == null)
        {
            throw new KeyNotFoundException($"Nota com ID {notaId} não encontrada.");
        }

        if (nota.Status == SharedStatusNota.Fechada)
        {
            throw new InvalidOperationException($"A nota {notaId} já está fechada e não pode ser processada novamente.");
        }

        nota.DataFechamento = DateTime.UtcNow;
        nota.Status = SharedStatusNota.Fechada;
        nota.EmProcessamento = true;
        await _context.SaveChangesAsync();

        await _eventPublisher.PublishAsync(new NotaFiscalProcessadaEvent
        {
            NotaFiscalId = nota.Id,
            Itens = nota.Itens.Select(i => new SharedNotaFiscalItem
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList()
        });

        return nota;
    }
    public async Task<Nota> ConcluirNotaAsync(int notaId, List<SharedNotaFiscalItemFalhou> eventItensFalhados, List<SharedNotaFiscalItem> eventItensProcessados)
    {
        var itensFalhados = new List<NotaFiscalItemFalhou>();
        var itensProcessados = new List<NotaFiscalItem>();

        var nota = await _context.Notas.FindAsync(notaId);

        if (nota == null)
        {
            throw new KeyNotFoundException($"Nota com ID {notaId} não encontrada.");
        }

        foreach (var item in eventItensFalhados)
        {
            itensFalhados.Add(new NotaFiscalItemFalhou
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                MotivoFalha = item.MotivoFalha,
                NotaId = notaId,
                Nota = nota
            });
        }

        foreach (var item in eventItensProcessados)
        {
            itensProcessados.Add(new NotaFiscalItem
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                Processado = true,
                NotaId = notaId,
                Nota = nota
            });
        }

        nota.ItensFalhados = itensFalhados;
        nota.ItensProcessados = itensProcessados;
        nota.EmProcessamento = false;

        await _context.SaveChangesAsync();

        return nota;
    }
}