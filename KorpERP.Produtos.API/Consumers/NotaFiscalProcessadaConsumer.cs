using MassTransit;
using KorpERP.Shared.Events;
using KorpERP.Produtos.API.Interfaces;

namespace KorpERP.Produtos.API.Consumers
{
    public class NotaFiscalProcessadaConsumer : IConsumer<NotaFiscalProcessadaEvent>
    {
        private readonly IProdutoService _produtoService;
        public NotaFiscalProcessadaConsumer(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        public Task Consume(ConsumeContext<NotaFiscalProcessadaEvent> context)
        {
            var message = context.Message;
            return _produtoService.ProcessarNotaFiscalAsync(message.NotaFiscalId, message.Itens.ToList());
        }
    }
}