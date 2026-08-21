using MassTransit;
using KorpERP.Shared.Events;
using KorpERP.Notas.API.Interfaces;

namespace KorpERP.Notas.API.Consumers
{
    public class ProdutoDesativadoConsumer : IConsumer<ProdutoDesativadoEvent>
    {
        private readonly IProdutoProjectionService _produtoProjectionService;
        public ProdutoDesativadoConsumer(IProdutoProjectionService produtoProjectionService)
        {
            _produtoProjectionService = produtoProjectionService;
        }

        public Task Consume(ConsumeContext<ProdutoDesativadoEvent> context)
        {
            var message = context.Message;
            return _produtoProjectionService.DesativarProdutoProjectionAsync(message.ProdutoId);
        }
    }
}