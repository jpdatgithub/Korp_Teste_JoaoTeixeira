using MassTransit;
using KorpERP.Shared.Events;
using KorpERP.Notas.API.Interfaces;

namespace KorpERP.Notas.API.Consumers
{
    public class ProdutoCriadoConsumer : IConsumer<ProdutoCriadoEvent>
    {
        private readonly IProdutoProjectionService _produtoProjectionService;
        public ProdutoCriadoConsumer(IProdutoProjectionService produtoProjectionService)
        {
            _produtoProjectionService = produtoProjectionService;
        }

        public Task Consume(ConsumeContext<ProdutoCriadoEvent> context)
        {
            var message = context.Message;
            return _produtoProjectionService.CreateProdutoProjectionAsync(message.ProdutoId, message.Codigo, message.Descricao, message.Versao);
        }
    }
}