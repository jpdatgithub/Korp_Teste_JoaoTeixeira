using MassTransit;
using KorpERP.Shared.Events;
using KorpERP.Notas.API.Interfaces;

namespace KorpERP.Notas.API.Consumers
{
    public class ProcessamentoDeNotaConcluidoConsumer : IConsumer<ProcessamentoDeNotaConcluidoEvent>
    {
        private readonly INotasService _notasService;
        public ProcessamentoDeNotaConcluidoConsumer(INotasService notasService)
        {
            _notasService = notasService;
        }

        public Task Consume(ConsumeContext<ProcessamentoDeNotaConcluidoEvent> context)
        {
            var message = context.Message;
            return _notasService.ConcluirNotaAsync(message.NotaFiscalId, message.ItensFalhos.ToList(), message.Itens.ToList());
        }
    }
}