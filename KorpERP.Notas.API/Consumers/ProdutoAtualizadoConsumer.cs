using MassTransit;
using KorpERP.Shared.Events;
using KorpERP.Notas.API.Interfaces;

namespace KorpERP.Notas.API.Consumers
{
    public class ProdutoAtualizadoConsumer : IConsumer<ProdutoAtualizadoEvent>
    {
        private readonly IProdutoProjectionService _produtoProjectionService;
        public ProdutoAtualizadoConsumer(IProdutoProjectionService produtoProjectionService)
        {
            _produtoProjectionService = produtoProjectionService;
        }

        public Task Consume(ConsumeContext<ProdutoAtualizadoEvent> context)
        {
            var message = context.Message;
            return _produtoProjectionService.AtualizarProdutoProjectionAsync(message.ProdutoId, message.Codigo, message.Descricao, message.Versao);
        }
    }
}