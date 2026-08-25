using MassTransit;
using KorpERP.Shared.Events;
using KorpERP.Notas.API.Interfaces;

namespace KorpERP.Notas.API.Consumers
{
    public class EstoqueAtualizadoConsumer : IConsumer<EstoqueAtualizadoEvent>
    {
        private readonly IProdutoProjectionService _produtoProjectionService;
        public EstoqueAtualizadoConsumer(IProdutoProjectionService produtoProjectionService)
        {
            _produtoProjectionService = produtoProjectionService;
        }

        public Task Consume(ConsumeContext<EstoqueAtualizadoEvent> context)
        {
            var message = context.Message;
            return _produtoProjectionService.AtualizarEstoqueAsync(message.ProdutoId, message.NovoSaldo, message.Versao);
        }
    }
}